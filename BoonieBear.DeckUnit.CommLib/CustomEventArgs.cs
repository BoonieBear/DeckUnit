using System;
using System.IO.Ports;

namespace BoonieBear.DeckUnit.CommLib
{
    public class CustomEventArgs:EventArgs
    {
        public CustomEventArgs(int id, string outstring, byte[] buf, int length, bool parseOK, string errorMsg, CallMode callmode, object callSrc,Exception e=null)
        {
            var src = callSrc as SerialPort;
            DataBufferLength = length;
            Ex = e;
            if (callmode == CallMode.DataMode&&src != null)//串口转发
            {

                if (length > 0)
                {
                    DataBuffer = new byte[length + 4];
                    UInt16 uid = 0xEE01; //与网络包格式一致
                    Buffer.BlockCopy(BitConverter.GetBytes(uid), 0, DataBuffer, 0, 2);
                    Buffer.BlockCopy(BitConverter.GetBytes(length), 0, DataBuffer, 2, 2);
                    Buffer.BlockCopy(buf, 0, DataBuffer, 4, length); //包头长度4，包括ID和长度
                    DataBufferLength = DataBuffer.Length;
                }

            }
            else
            {
                if (length>0)
                {
                    DataBuffer = new byte[length];
                    Buffer.BlockCopy(buf, 0, DataBuffer, 0, length);
                        
                }
            }
            ID = id;
            Outstring = outstring;
            
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
        public Exception Ex { get; private set; }
        public string Outstring { get; set; }
        public int DataBufferLength { get; private set; }
    }
}
