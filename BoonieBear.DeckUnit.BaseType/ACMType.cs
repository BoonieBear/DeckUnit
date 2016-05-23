using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoonieBear.DeckUnit.BaseType
{
    public class CommType
    {
        public string Comm { get; set; }
        public int Baud { get; set; }
        public int DataBits { get; set; }
        public int Parity { get; set; }
        public int StopBits { get; set; }

    }

    public class CommConfInfo
    {
        public string SerialPort { get; set; }
        public CommType BPComm { get; set; }
        public CommType ADCPComm { get; set; }
        public int SerialPortRate { get; set; }
        public int NetPort1 { get; set; }
        public int NetPort2 { get; set; }
        public string LinkIP { get; set; }
        public int TraceUDPPort { get; set; }
        public int DataUDPPort { get; set; }

    }

    public class ModemConfigure
    {
        public int ID { get; set; }
        public int TransmiterType { get; set; }
        public int TransducerNum { get; set; }
        //public int ModemType { get; set; }
        public int Com2Device { get; set; }
        public int Com3Device { get; set; }
        public bool NetSwitch { get; set; }
        public int NodeType { get; set; }
        public int AccessMode { get; set; }

    }

    public class MovConfInfo
    {
        public int Mode { get; set; }
        public int SailPort { get; set; }
        public int GPSPort { get; set; }
        public int USBLPort { get; set; }
        public int BroadCastPort { get; set; }

    }

    public struct OASSEND
    {
        public int direction;
        public int id;
    }
}
