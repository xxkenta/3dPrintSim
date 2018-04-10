using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrinterSimulator
{
    public class GcodeParser
    {

        public double xVoltage = 0;
        public double yVoltage = 0;
        public bool laserOn = false;
        public bool moveBuildPlate = false;
        public double zRailMovement = 0;
        public double prevZRail = 0;
        public StreamReader GcodeFile;

        public GcodeParser(string filename)
        {
            this.GcodeFile = new StreamReader(filename);
        }

        public static string GetFilePath()
        {
            string path = "file";

            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                path = file.FileName;
            }

            return path;
        }

        public double GetXVoltage()
        {
            return this.xVoltage;
        }

        public double GetYVoltage()
        {
            return this.yVoltage;
        }

        public bool GetLaserOn()
        {
            return this.laserOn;
        }

        public bool GetMoveBuildPlate()
        {
            return this.moveBuildPlate;
        }

        public double GetZRailMovement()
        {
            return this.zRailMovement;
        }

        public void ParseGcodeLine(StreamReader gcodeFile)
        {
            string line;

            if ((line = gcodeFile.ReadLine()) != null)
            {
                while (true)
                {
                    this.prevZRail = this.zRailMovement;

                    if (line.StartsWith("G1"))
                    { 
                        //set these variables to 9999 so host can tell the difference between an galvo move command and a zrail move command
                        this.xVoltage = 9999;
                        this.yVoltage = 9999;
                        this.zRailMovement = 9999;
                        this.moveBuildPlate = false;

                        Console.WriteLine(line);
                        string[] words = line.Split(' ');
                        foreach (var word in words)
                        {
                            if (word.StartsWith("X"))
                            {
                                this.xVoltage = Convert.ToDouble(word.Substring(1));
                            }
                            if (word.StartsWith("Y"))
                            {
                                this.yVoltage = Convert.ToDouble(word.Substring(1));
                            }
                            if (word.StartsWith("Z"))
                            {
                                this.zRailMovement = Convert.ToDouble(word.Substring(1));
                                this.moveBuildPlate = true;
                            }
                            if (word.StartsWith("E"))
                            {
                                if (Convert.ToDouble(word.Substring(1)) > 0)
                                {
                                    this.laserOn = true;
                                }
                            }
                        }
                        
                        break;
                    }
                    else if (line.StartsWith("G92"))
                    {
                        this.laserOn = false;
                        break;
                    }
                    else
                    {
                        line = gcodeFile.ReadLine();
                        if ((line = gcodeFile.ReadLine()) != null)
                        {
                            break;
                        }
                    }

                }
            }
        }
    }   
}
