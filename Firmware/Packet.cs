using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hardware;

namespace Firmware
{
    public class Packet
    {
        public byte cmd;
        public int length;
        public ushort checksum;
        public byte[] data;

        public Packet(byte cmd, byte[] data)
        {
            this.cmd = cmd;
            this.data = data;
            this.length = data.Length;
            this.checksum = FindChecksum(cmd, data, length);
        }

        public byte [] GetHeader()
        {
            byte[] header = {cmd, (byte) length, BitConverter.GetBytes(checksum)[0], BitConverter.GetBytes(checksum)[1]};

            return header;
        }

        public ushort FindChecksum(byte cmd, byte[] data, int length)
        {
            //Console.WriteLine("\nHost Checksum Calculation: ");
            //Console.WriteLine("CMD: " + cmd);
            //Console.WriteLine("Data: ");
            
            checksum = 0;
            foreach (byte element in data)
            {
                this.checksum += element;
                //Console.Write(" ");
                //Console.Write(element);
            }
            //Console.WriteLine("\nLength: " + length);
            checksum += cmd;
            checksum += (byte) length;

            return checksum;
        }

        public static Packet LaserOn(bool onOff)
        {
            return new Packet((byte) Cmds.LASER, BitConverter.GetBytes(onOff));
        }

        public static Packet MoveGalvos(double x, double y)
        {
            x = x * 0.025; //converts coordinate to voltage
            y = y * 0.025; //converts coordinate to voltage
            byte[] xVolt = BitConverter.GetBytes(x);
            byte[] yVolt = BitConverter.GetBytes(y);
            List<byte> list1 = new List<byte>(xVolt);
            List<byte> list2 = new List<byte>(yVolt);
            list1.AddRange(list2);
            byte[] volts = list1.ToArray();
            return new Packet((byte) Cmds.GALVOS, volts);
        }

        public static Packet MoveZ(double prevZ, double newZ)
        {
            //Console.WriteLine("New Z: " + newZ);
            //Console.WriteLine("Old Z: " + prevZ);
            double z = newZ - prevZ;
            byte[] zcor = BitConverter.GetBytes(z);
            return new Packet((byte)Cmds.ZCOR, zcor);
        }

        public Packet Reset()
        {
            byte[] top = { (byte)0 };
            return new Packet((byte)Cmds.RESET, top);
        }


        public enum Cmds
        {
            LASER = 0,
            GALVOS = 1,
            ZCOR = 2,
            RESET = 3
        }
    }
}
