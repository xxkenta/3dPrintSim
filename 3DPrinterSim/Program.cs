using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Hardware;
using Firmware;
using System.Windows.Forms;

namespace PrinterSimulator
{
    class PrintSim
    {
        static void PrintFile(PrinterControl simCtl)
        {
            //System.IO.StreamReader file = new System.IO.StreamReader("..\\..\\..\\SampleSTLs\\F-35_Corrected.gcode");
            string path = "file";
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                path = file.FileName;
            }

            Stopwatch swTimer = new Stopwatch();
            swTimer.Start();

            // Todo - Read GCODE file and send data to firmware for printing
            Loop(simCtl);

            swTimer.Stop();
            long elapsedMS = swTimer.ElapsedMilliseconds;

            Console.WriteLine("Total Print Time: {0}", elapsedMS / 1000.0);
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

        static void Loop(PrinterControl simCtl) //Makes it so the function repeats recursively until it is properly sent
        {
            byte cmd;
            byte len;
            byte lowCheck;
            byte highCheck;
            byte[] packetHeader = { cmd, len, lowCheck, highCheck };
            byte[] ack = { 0xA5 };
            byte[] nack = { 0xFF };
            byte[] packet = {}; //Assign later

            int sent = simCtl.WriteSerialToFirmware(packetHeader, 4); //Send 4 byte header
            int recieved = simCtl.ReadSerialFromFirmware(firmHeader, 4);//Read 4 byte response (MAKE SURE TO DECLARE FIRMHEADER IN THE FIRMWARE)

            if (sent == recieved) //Check the the firmware recieved the correct header
            {
                simCtl.WriteSerialToFirmware(ack, 1); //Send Ack
                simCtl.WriteSerialToFirmware(packet, len - 4); //Send Rest of packet

            }
            else
            {
                simCtl.WriteSerialToFirmware(nack, 1); //Send Nack
                Loop(simCtl); //Try again until successful
            }
        }

        [STAThread]

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [STAThread]
        static void Main()
        {

            IntPtr ptr = GetConsoleWindow();
            MoveWindow(ptr, 0, 0, 1000, 400, true);

            // Start the printer - DO NOT CHANGE THESE LINES
            PrinterThread printer = new PrinterThread();
            Thread oThread = new Thread(new ThreadStart(printer.Run));
            oThread.Start();
            printer.WaitForInit();

            // Start the firmware thread - DO NOT CHANGE THESE LINES
            FirmwareController firmware = new FirmwareController(printer.GetPrinterSim());
            oThread = new Thread(new ThreadStart(firmware.Start));
            oThread.Start();
            firmware.WaitForInit();

            SetForegroundWindow(ptr);

            bool fDone = false;
            while (!fDone)
            {
                Console.Clear();
                Console.WriteLine("3D Printer Simulation - Control Menu\n");
                Console.WriteLine("P - Print");
                Console.WriteLine("T - Test");
                Console.WriteLine("Q - Quit");

                char ch = Char.ToUpper(Console.ReadKey().KeyChar);
                switch (ch)
                {

                    case 'P': // Print
                        PrintFile(printer.GetPrinterSim());
                        break;

                    case 'T': // Test menu
                        Console.Clear();
                        Console.WriteLine("3D Printer Simulation - Test Menu\n");
                        Console.WriteLine("B - Test build plate movement from top to bottom");
                        Console.WriteLine("G - Test galvo");
                        Console.WriteLine("L - Test laser on/off");
                        Console.WriteLine("T - Test host to firmware connection");
                        Console.WriteLine("Z - Test build plate movement to specific point");

                        ch = Char.ToUpper(Console.ReadKey().KeyChar);
                        switch (ch)
                        {
                            case 'B': // Test build plate movement from top to bottom
                                //add functionalty
                                break;

                            case 'G': // Test galvo
                                //add functionalty
                                break;

                            case 'L': // Test laser on/off
                                //add functionalty
                                break;

                            case 'T': // Test host to firmware connection
                                //add functionalty
                                break;

                            case 'Z': // Test build plate movement to specific point
                                //add functionalty
                                break;
                        }
                        break;

                    case 'Q' :  // Quite
                        printer.Stop();
                        firmware.Stop();
                        fDone = true;
                        break;
                }
            }
        }
    }
}