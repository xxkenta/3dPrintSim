using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hardware;

namespace PrinterSimulator
{
    public class Packet
    {
        public byte cmd;
        public int Length;
        public ushort checksum = 0;
        public byte[] data;

        public Packet(byte cmd, byte[] data)
        {
            this.cmd = cmd;
            this.data = data;
            this.Length = data.Length;
            findChecksum();
        }

        public byte [] getHeader()
        {
            byte[] header = {cmd, (byte) Length, BitConverter.GetBytes(checksum)[0], BitConverter.GetBytes(checksum)[1]};

            return header;
        }

        public void findChecksum()
        {
            
        }

        public Packet LaserOn(bool onOff)
        {
            return new Packet((byte) cmds.LASER, BitConverter.GetBytes(onOff));
        }

        public Packet moveGalvos(float x, float y)
        {
            byte xCor = (byte) x;
            byte yCor = (byte)y;
            byte[] cors = { xCor, yCor };
            return new Packet((byte) cmds.GALVOS, cors);
        }

        public Packet moveZ(float z)
        {
            byte[] zcor = { (byte)z };
            return new Packet((byte)cmds.ZCOR, zcor);
        }

        public Packet reset()
        {
            byte[] top = { (byte)0 };
            return new Packet((byte)cmds.RESET, top);
        }



        public enum cmds
        {
            LASER = 0,
            GALVOS = 1,
            ZCOR = 2,
            RESET = 3
        }
    }
}
