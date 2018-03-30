using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hardware;
using Firmware;
using PrinterSimulator;

namespace PrinterSimulator
{
    public class CommunicationsProtocol
    {
        public void sendPacket(PrinterControl printer, Packet packet, FirmwareController firmware)
        {
            byte[] header = packet.getHeader();
            printer.WriteSerialToFirmware(header, header.Length);
            byte[] responseHeader = firmwareReadPacket(printer, header.Length);

            if(sameHead(header, responseHeader) == true)
            {
                printer.WriteSerialToFirmware(new byte[] { 0xA5 }, 1);
                printer.WriteSerialToFirmware(packet.data, packet.data.Length);

                byte[] character = firmwareReadPacket(printer, 1);
                string response = "";
                while(character[0] != 0)
                {
                    response += ASCIIEncoding.ASCII.GetString(new byte[] { character[0] });
                    character = firmwareReadPacket(printer, 1);
                }

                if(response == "SUCCESS")
                {
                    return;
                }
                sendPacket(printer, packet, firmware);
            }
            printer.WriteSerialToFirmware(new byte[] { 0xFF }, 1);
            sendPacket(printer, packet, firmware);
            return;
        }



        public byte[] firmwareReadPacket(PrinterControl printer, int expected) 
        {
            byte[] data = new byte[expected];
            printer.ReadSerialFromHost(data, expected);
            return data;
        }



        public void processCmd(Packet packet, FirmwareController firmware)
        {
            
        }

        public bool sameHead(byte[] h1, byte[] h2)
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
    }
}
