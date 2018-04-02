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
            //System.IO.StreamReader file = new System.IO.StreamReader("..\\..\\..\\SampleSTLs\\F-35_Corrected.gcode");
            GcodeParser parser = new GcodeParser(GcodeParser.GetFilePath());
            for (int i = 0; i < 100; i++)
            {
                parser.ParseGcodeLine(parser.GcodeFile);
            }





            Stopwatch swTimer = new Stopwatch();
            swTimer.Start();

            // Todo - Read GCODE file and send data to firmware for printing
            //Loop(simCtl);

            swTimer.Stop();
            long elapsedMS = swTimer.ElapsedMilliseconds;

            Console.WriteLine("Total Print Time: {0}", elapsedMS / 1000.0);
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
<<<<<<< HEAD

=======
>>>>>>> 8c3a17fafae59043585763d60d825d00c09be410

      
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
                        PrintFile(printer.GetPrinterSim());
                        break;

                    case 'T': // Test menu
                        Console.Clear();
                        Console.WriteLine("3D Printer Simulation {0} - Test Menu\n", (object) firmware.GetVer());
                        Console.WriteLine("B - Test build plate movement from top to bottom");
                        Console.WriteLine("G - Test galvo");
                        Console.WriteLine("L - Test laser on/off");
                        Console.WriteLine("T - Test host to firmware connection");
                        Console.WriteLine("Z - Test build plate movement to specific point");
                        Console.WriteLine("Q - Back");

                        ch = Char.ToUpper(Console.ReadKey().KeyChar);
                        switch (ch)
                        {
                            case 'B': // Test build plate movement from top to bottom
                                firmware.SetBuildPlateHome();
                                break;

                            case 'G': // Test galvo
                                //add functionalty
                                break;

                            case 'L': // Test laser on/off
                                Console.WriteLine("1 - On");
                                Console.WriteLine("0 - Off");

                                int x = Console.Read();
                                switch(x)
                                {
                                    case '1': // Test Laser On
                                        firmware.TurnLaserOn();
                                        break;
                                    
                                    case '0': // Test Laser Off
                                        firmware.TurnLaserOff();
                                        break;
                                }
                                break;

                            case 'T': // Test host to firmware connection
                                //add functionalty
                                break;

                            case 'Z': // Test build plate movement to specific point
                                Console.WriteLine("What position on the Z-rail would you like to move the build plate? (0-100): ");
                                int pos = Convert.ToInt32(Console.ReadLine());
                                int steps = 400*pos;
                                firmware.SetBuildPlateHome();
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