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
        public int Length;
        public ushort checksum;
        public byte[] data;

        public Packet(byte cmd, byte[] data)
        {
            this.cmd = cmd;
            this.data = data;
            this.Length = data.Length;
            FindChecksum();
        }

        public byte [] GetHeader()
        {
            byte[] header = {cmd, (byte) Length, BitConverter.GetBytes(checksum)[0], BitConverter.GetBytes(checksum)[1]};

            return header;
        }

        public void FindChecksum()
        {
            
        }

        public static Packet LaserOn(bool onOff)
        {
            return new Packet((byte) Cmds.LASER, BitConverter.GetBytes(onOff));
        }

        public static Packet MoveGalvos(double x, double y)
        {
            x = x * 0.025; //converts coordinate to voltage
            y = y * 0.025; //converts coordinate to voltage
            byte xVolt = (byte) x;
            byte yVolt = (byte) y;
            byte[] volts = { xVolt, yVolt };
            return new Packet((byte) Cmds.GALVOS, volts);
        }

        public static Packet MoveZ(double prevZ, double newZ)
        {
            double z = prevZ - newZ;
            byte[] zcor = { (byte)z };
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
