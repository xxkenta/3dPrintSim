using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Drawing;
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
            GcodeParser parser = new GcodeParser(GcodeParser.GetFilePath());

            Stopwatch swTimer = new Stopwatch();
            swTimer.Start();
            
            while (!parser.GcodeFile.EndOfStream)
            {
                parser.ParseGcodeLine(parser.GcodeFile);

                if(parser.prevLaserOn != parser.laserOn)
                {
                    Packet laser = Packet.LaserOn(parser.laserOn);
                    CommunicationsProtocol.SendPacket(simCtl, laser);
                }
                if (parser.xVoltage != 9999 && parser.yVoltage != 9999) // checks for valid x,y coordinates from parser
                {
                    Packet galv = Packet.MoveGalvos(parser.xVoltage, parser.yVoltage);
                    CommunicationsProtocol.SendPacket(simCtl, galv);
                }
                if (parser.moveBuildPlate) // checks if GCODE command was to move zrail
                {
                    Packet Z = Packet.MoveZ(parser.prevZRail, parser.zRailMovement);
                    CommunicationsProtocol.SendPacket(simCtl, Z);
                }
            }
            swTimer.Stop();
            long elapsedMS = swTimer.ElapsedMilliseconds;

            Console.WriteLine("Total Print Time: {0}", elapsedMS / 1000.0);
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
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
                Console.WriteLine("3D Printer Simulation {0} - Control Menu\n", (object) firmware.GetVer());
                Console.WriteLine("P - Print");
                Console.WriteLine("T - Test");
                Console.WriteLine("Q - Quit");

                char ch = Char.ToUpper(Console.ReadKey().KeyChar);
                switch (ch)
                {

                    case 'P': // Print
                        Packet setPlateHome = Packet.Reset();
                        CommunicationsProtocol.SendPacket(printer.GetPrinterSim(), setPlateHome);
                        PrintFile(printer.GetPrinterSim());
                        break;

                    case 'T': // Test menu
                        Console.Clear();
                        Console.WriteLine("3D Printer Simulation {0} - Test Menu\n", (object) firmware.GetVer());
                        Console.WriteLine("B - Test build plate movement from top to bottom");
                        Console.WriteLine("G - Test galvo");
                        Console.WriteLine("L - Test laser on/off");
                        Console.WriteLine("Z - Test build plate movement to specific point");
                        Console.WriteLine("Q - Back");

                        ch = Char.ToUpper(Console.ReadKey().KeyChar);
                        switch (ch)
                        {
                            case 'B': // Test build plate movement from top to bottom
                                Packet reset = Packet.Reset();
                                CommunicationsProtocol.SendPacket(printer.GetPrinterSim(), reset);
                                break;

                            case 'G': // Test galvo
                                Console.WriteLine("Insert the X and Y coordinate separated by a space: ");

                                string[] tokens = Console.ReadLine().Split();

                                //Parse element 0
                                double xVoltage = double.Parse(tokens[0]);

                                //Parse element 1
                                double yVoltage = double.Parse(tokens[1]);

                                Packet galv = Packet.MoveGalvos(xVoltage, yVoltage);
                                CommunicationsProtocol.SendPacket(printer.GetPrinterSim(), galv);
                                Packet galv2 = Packet.MoveGalvos(xVoltage + 10, yVoltage+10);
                                CommunicationsProtocol.SendPacket(printer.GetPrinterSim(), galv2);

                                break;

                            case 'L': // Test laser on/off
                                Console.WriteLine("1 - On");
                                Console.WriteLine("0 - Off");

                                string x = Console.ReadLine();
                                switch(x)
                                {
                                    case "1": // Test Laser On
                                        Packet laserOn = Packet.LaserOn(true);
                                        CommunicationsProtocol.SendPacket(printer.GetPrinterSim(), laserOn);
                                
                                        break;
                                    
                                    case "0": // Test Laser Off
                                        Packet laserOff = Packet.LaserOn(false);
                                        CommunicationsProtocol.SendPacket(printer.GetPrinterSim(), laserOff);
                                        break;
                                }
                                break;

                            case 'Z': // Test build plate movement to specific point
                                Console.WriteLine("What position on the Z-rail would you like to move the build plate? (0-100): ");
                                int pos = Convert.ToInt32(Console.ReadLine());
                                int steps = 400*pos;
                                Packet testZrail = Packet.Reset();
                                CommunicationsProtocol.SendPacket(printer.GetPrinterSim(), testZrail);
                                firmware.StepStepperUp(steps);
                                break;

                            case 'Q': //back to main menu
                                break;
                        }
                        break;

                    case 'Q' :  // Quit
                        printer.Stop();
                        firmware.Stop();
                        fDone = true;
                        break;
                }
            }
        }
    }
}