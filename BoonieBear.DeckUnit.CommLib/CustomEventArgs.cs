using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.CommLib.Serial;
namespace BoonieBear.DeckUnit.CommLib
{
    public class CustomEventArgs
    {
        public CustomEventArgs(string outstring, byte[] buf, int length, bool parseOK, string errorMsg, CallMode callmode)
        {
            if (length > 0)
            {
                DataBuffer = new byte[length];
                Buffer.BlockCopy(buf, 0, DataBuffer, 0, length);
            }
            Outstring = outstring;
            DataBufferLength = length;
            ParseOK = parseOK;
            ErrorMsg = errorMsg;
            Mode = callmode;
                
        }

        public bool ParseOK { get; private set; }
        public string ErrorMsg { get; private set; }
        public CallMode Mode { get; private set; }
        public byte[] DataBuffer { get; private set; }

        public string Outstring { get; set; }
        public int DataBufferLength { get; private set; }
    }
}
