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

        public Packet LaserOn(bool onOff)
        {
            return new Packet((byte) Cmds.LASER, BitConverter.GetBytes(onOff));
        }

        public Packet MoveGalvos(float x, float y)
        {
            byte xCor = (byte) x;
            byte yCor = (byte)y;
            byte[] cors = { xCor, yCor };
            return new Packet((byte) Cmds.GALVOS, cors);
        }

        public Packet MoveZ(float prevZ, float newZ)
        {
            float z = prevZ - newZ;
            byte[] zcor = { (byte)z };
            return new Packet((byte)Cmds.ZCOR, zcor);
        }

        public Packet Reset()
        {
            byte[] top = { (byte)0 };
            return new Packet((byte)Cmds.RESET, top);
        }

    }
}
