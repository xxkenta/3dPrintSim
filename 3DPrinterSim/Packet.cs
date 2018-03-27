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
    }
}
