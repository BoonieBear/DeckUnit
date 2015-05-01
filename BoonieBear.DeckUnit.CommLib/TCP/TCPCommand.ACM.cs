using System;
using System.Net.Sockets;
using System.Threading;

namespace BoonieBear.DeckUnit.CommLib.TCP
{
    class ACMTCPCommand:TCPBaseComm
    {
        private CustomEventArgs _args = new CustomEventArgs(null, null, 0, false, null, CallMode.NoneMode,null);
        private static readonly AutoResetEvent EAutoResetEvent = new AutoResetEvent(false);
        private const int TimeOut = 10000;
        private static readonly object Lockobject = new object();
        public ACMTCPCommand(TcpClient tcpClient, byte[] bytes)
        {
            if (!base.Init(tcpClient)) return;
            if (bytes==null) return;
            base.LoadData(bytes);
        }

        public override bool Send(out string error)
        {
            var flag = false;
            try
            {
                if (EAutoResetEvent.WaitOne(TimeOut))
                {

                    if (!_args.ParseOK)
                    {
                        error = _args.ErrorMsg;
                        return false ;
                    }
                    error = String.Empty;
                    return true;
                }
                error = " 接收数据超时！";
                return false;
            }
            catch (Exception exception)
            {
                error = exception.Message;

                return false;
            }
           
        }
        

        public override void Handle(object sender, CustomEventArgs e)
        {
            lock (Lockobject)
            {
                _args = e;
                if ((_args.Mode.Equals(CallMode.DataMode)) && (BitConverter.ToUInt16(_args.DataBuffer, 0) == 0x45AC))
                {

                    EAutoResetEvent.Set();

                }
            }
        }
    }
}
