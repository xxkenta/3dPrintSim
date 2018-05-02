using System;
using System.Text;
using Firmware;
using Hardware;

namespace PrinterSimulator
{
    public class CommunicationsProtocol
    {
        public static void SendPacket(PrinterControl printer, Packet packet)
        {
            byte[] header = packet.GetHeader();
            byte[] headerCopy = new byte[4];
            header.CopyTo(headerCopy, 0);

            int result = 0;
            //Console.WriteLine("\nHost sending to firmware: ");
            //for (int i = 0; i < header.Length; i++)
            //{
            //    Console.Write(" ");
            //    Console.Write(header[i]);
            //}
            result = printer.WriteSerialToFirmware(headerCopy, header.Length);

            byte[] responseHeader = ReadPacket(printer, header.Length);

            if (SameHead(header, responseHeader) == true)
            {
                //Console.WriteLine("\nHost sent ACK to firmware: ");
                printer.WriteSerialToFirmware(new byte[] { 0xA5 }, 1);

                //Console.WriteLine("\nHost sending packet to firmware: ");
                //for (int i = 0; i < packet.data.Length; i++)
                //{
                //    Console.Write(" ");
                //    Console.Write(packet.data[i]);
                //}
                byte[] packetDataCopy = new byte[packet.data.Length];
                packet.data.CopyTo(packetDataCopy, 0);

                printer.WriteSerialToFirmware(packetDataCopy, packet.data.Length);

                byte[] character = ReadPacket(printer, 1);
                string response = "";
                while (character[0] != 0)
                {
                    response += Encoding.ASCII.GetString(new byte[] { character[0] });
                    character = ReadPacket(printer, 1);
                }
                //Console.WriteLine("\nResponse: " + response);
                if (response == "SUCCESS")
                {
                    Console.WriteLine("Success");
                    return;
                }
                else if (response.Contains("VERSION"))
                {
                    return;
                }
                else
                {
                    Console.WriteLine("Failure");
                    SendPacket(printer, packet);
                }
            }
            else
            {
                Console.WriteLine("2 failure");
                printer.WriteSerialToFirmware(new byte[] { 0xFF }, 1);
                SendPacket(printer, packet);
            }
            return;
        }

        public static byte[] ReadPacket(PrinterControl printer, int expected) 
        {
            byte[] data = new byte[expected];
            byte[] failure = new byte[4];
            int count = 0;
            while (count < 1000000000)
            {
                int response = printer.ReadSerialFromFirmware(data, expected);
                if (response == expected)
                {
                    //Console.WriteLine("\nHost is reading: ");
                    //for (int i = 0; i < data.Length; i++)
                    //{
                    //    Console.Write(" ");
                    //    Console.Write(data[i]);
                    //}
                    return data;
                }
                count++;
            }
            return failure;
        }


        public static bool SameHead(byte[] h1, byte[] h2)
        {
            //Console.WriteLine("\n-- Checking Header is Same --");
            //Console.WriteLine("H1 Length: " + h1.Length);
            //Console.WriteLine("H2 Length: " + h2.Length);
            //Console.WriteLine("H1 contents: ");
            //for (int i = 0; i < h1.Length; i++)
            //{
            //    Console.Write(" ");
            //    Console.Write(h1[i]);
            //}
            //Console.WriteLine("\nH2 contents: ");
            //for (int i = 0; i < h2.Length; i++)
            //{
            //    Console.Write(" ");
            //    Console.Write(h2[i]);
            //}
            //Console.WriteLine(" ");

            if (h1.Length != h2.Length)
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
