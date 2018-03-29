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
        PrinterControl control;
        Packet printer;
        byte[] buffer = new byte[32];

        public CommunicationsProtocol()
        {
            int header1 = control.WriteSerialToHost(printer.getHeader(), printer.Length);
            int header2 = control.ReadSerialFromHost(printer.getHeader(), printer.Length);

            if (header1 != header2)
            {
                control.WriteSerialToFirmware(buffer, 0xFF);
            }

        }
    }
}
