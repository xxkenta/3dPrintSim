using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hardware;
//using PrinterSimulator;


namespace Firmware
{

    public class FirmwareController
    {
        PrinterControl printer;
        bool fDone = false;
        bool fInitialized = false;
        string version = "v0.1";
        ZRailController zRailController;

        public FirmwareController(PrinterControl printer)
        {
            this.printer = printer;
            zRailController = new ZRailController(printer);
        }

        public string GetVer()
        {
            return this.version;
        }
        // Handle incoming commands from the serial link
        void Process()
        {
            // Todo - receive incoming commands from the serial link and act on those commands by calling the low-level hardwarwe APIs, etc.
            while (!fDone)
            {
                byte[] header = ReadPacket(printer, 4);
                byte length = header[1];
                byte[] response;

                if (header.Any(b => b != 0))
                {
                    printer.WriteSerialToHost(header, 4);
                }

                byte[] ack = ReadPacket(printer, 1);
                if(ack[0] == 0xA5)
                {
                    byte[] data = ReadPacket(printer, length);
                    if(data.Length == 0)
                    {
                        response = Encoding.ASCII.GetBytes("Timeout");
                    }
                    else
                    {
                        response = ProcessCmd(header[0], data);
                    }
                    //printer.WriteSerialToHost(response, response.Length);
                    for (int i = 0; i < response.Length; i++)
                    {
                        byte[] character = { response[i] };
                        printer.WriteSerialToHost(character, 1);
                    }
                    byte[] nullByte = { (byte)0 };
                    printer.WriteSerialToHost(nullByte, 1);
                }

            }
        }

        public void Start()
        {
            fInitialized = true;

            Process(); // this is a blocking call
        }

        public void Stop()
        {
            fDone = true;
        }

        public void WaitForInit()
        {
            while (!fInitialized)
                Thread.Sleep(100);
        }

        public byte[] ReadPacket(PrinterControl printer, int expected)
        {
            byte[] data = new byte[expected];
            byte[] failure = new byte[4];
            int response = 0;
            while (response != expected)
            {
                response = printer.ReadSerialFromHost(data, expected);
            }
            return data;
        }

        public byte[] ProcessCmd(byte cmd, byte[] data)
        {
            if(cmd == (byte) Packet.Cmds.LASER)
            {
                printer.SetLaser(BitConverter.ToBoolean(data, 0));
            }
            else if(cmd == (byte) Packet.Cmds.GALVOS)
            {
                printer.MoveGalvos((float)BitConverter.ToDouble(data, 0), (float)BitConverter.ToDouble(data, 8));
            }
            else if(cmd == (byte) Packet.Cmds.ZCOR) // Find more efficient method
            { 
                Console.WriteLine(BitConverter.ToDouble(data,0));
                if (((float) data[0]) < 0)
                {
                    StepStepperDown((int)(400 * BitConverter.ToDouble(data, 0)));
                }
                else
                {
                    StepStepperUp((int)(400 * BitConverter.ToDouble(data, 0)));
                }
            }
            else if(cmd == (byte) Packet.Cmds.RESET)
            {
                SetBuildPlateHome();
            }
            return Encoding.ASCII.GetBytes("SUCCESS");
        }

        //Since the build plate starts at a random point every time, this function moves it all the way up until
        //the limit switch is pressed. It then steps down 39780 (calculated from datasheet) which will put it at the home position.
        public void SetBuildPlateHome()
        {

            while (printer.LimitSwitchPressed() != true)
            {
                zRailController.StepOnce(PrinterControl.StepperDir.STEP_UP);
            }

            zRailController.ResetSteps();

            Console.WriteLine("Build plate is at top and will now move to bottom");

            zRailController.MoveZRail(PrinterControl.StepperDir.STEP_DOWN, 39780);
        }

        public void StepStepperUp(int steps)
        {
            int stepCount = 0;
            for (int i = 0; i < steps; i++)
            {
                printer.StepStepper(PrinterControl.StepperDir.STEP_UP);
                stepCount++;
                if (stepCount == 40)
                {
                    Thread.Sleep(20);
                    stepCount = 0;
                }
            }
        }

        public void StepStepperDown(int steps)
        {
            int stepCount = 0;
            for (int i = 0; i < steps; i++)
            {
                printer.StepStepper(PrinterControl.StepperDir.STEP_DOWN);
                stepCount++;
                if (stepCount == 40)
                {
                    Thread.Sleep(20);
                    stepCount = 0;
                }
            }
        }
        public void TurnLaserOn()
        {
            printer.SetLaser(true);
        }
        public void TurnLaserOff()
        {
            printer.SetLaser(false);
        }
    }
}
