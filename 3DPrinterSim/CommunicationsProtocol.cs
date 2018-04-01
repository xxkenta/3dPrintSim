using System;
using System.Text;
using Hardware;

namespace PrinterSimulator
{
    public class CommunicationsProtocol
    {
        public void SendPacket(PrinterControl printer, Packet packet)
        {
            byte[] header = packet.GetHeader();
            printer.WriteSerialToFirmware(header, header.Length);
            byte[] responseHeader = ReadPacket(printer, header.Length);

            if(SameHead(header, responseHeader) == true)
            {
                printer.WriteSerialToFirmware(new byte[] { 0xA5 }, 1);
                printer.WriteSerialToFirmware(packet.data, packet.data.Length);

                byte[] character = ReadPacket(printer, 1);
                string response = "";
                while(character[0] != 0)
                {
                    response += Encoding.ASCII.GetString(new byte[] { character[0] });
                    character = ReadPacket(printer, 1);
                }

                if(response == "SUCCESS")
                {
                    return;
                }
                if(response.Contains("VERSION"))
                {
                    return;
                }

                SendPacket(printer, packet);
            }
            printer.WriteSerialToFirmware(new byte[] { 0xFF }, 1);
            SendPacket(printer, packet);
            return;
        }




        public byte[] ReadPacket(PrinterControl printer, int expected) 
        {
            byte[] data = new byte[expected];
            printer.ReadSerialFromFirmware(data, expected);
            return data;
        }


        public bool SameHead(byte[] h1, byte[] h2)
        {
            if(h1.Length != h2.Length)
            {
                return false;
            }
            for (int i = 0; i < h1.Length; i++)
            {
                if(h1[i] != h2[i])
                {
                    return false;
                }
            }
            return true;
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
