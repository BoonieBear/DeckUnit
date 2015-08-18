using System;

namespace BoonieBear.DeckUnit.CommLib
{
    public class CustomEventArgs:EventArgs
    {
        public CustomEventArgs(int id, string outstring, byte[] buf, int length, bool parseOK, string errorMsg, CallMode callmode, object callSrc)
        {
            if (length > 0)
            {
                DataBuffer = new byte[length+4];
                Buffer.BlockCopy(buf, 0, DataBuffer, 0, 4+length);//包头长度4，包括ID和长度
            }
            ID = id;
            Outstring = outstring;
            DataBufferLength = length;
            ParseOK = parseOK;
            ErrorMsg = errorMsg;
            Mode = callmode;
            CallSrc = callSrc;
        }
        public int ID { get; private set; }
        public bool ParseOK { get; private set; }
        public string ErrorMsg { get; private set; }
        public CallMode Mode { get; private set; }
        public object CallSrc { get; set; }
        public byte[] DataBuffer { get; private set; }

        public string Outstring { get; set; }
        public int DataBufferLength { get; private set; }
    }
}
