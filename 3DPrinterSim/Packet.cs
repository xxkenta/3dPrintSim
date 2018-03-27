using System;
namespace PrinterSimulator
{
    public class Packet
    {
        public byte cmd;
        public int Length;
        public int checksum = 0;
        public byte[] data;

        public Packet(byte cmd, byte[] data)
        {
            this.cmd = cmd;
            this.data = data;
            this.Length = data.Length;
            findChecksum();
        }

        public void findChecksum()
        {
            
        }
    }
}
