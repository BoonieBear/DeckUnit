using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using BoonieBear.DeckUnit.ACNP;
namespace BoonieBear.DeckUnit.CommLib.Serial
{
    public class ACNSerialHexCommand : SerialBaseComm
    {
        private  CustomEventArgs _args = new CustomEventArgs(0, null,null,0,false,null,CallMode.NoneMode,null);
        private static readonly ReaderWriterLock MReaderWriterLock = new ReaderWriterLock();
        private static readonly AutoResetEvent EAutoResetEvent = new AutoResetEvent(false);
        private static readonly AutoResetEvent DAutoResetEvent = new AutoResetEvent(false);
        private static bool _haveReceData = false;
        private const int TimeOut = 2000;
        private static readonly object Lockobject = new object();
        public ACNSerialHexCommand(SerialPort serialPort,byte[] bytes)
        {
            _serialPort = serialPort;

            _nBytes = bytes;

        }
  
        public override bool Send(out string error)
        {
            try
            {
                if (SendData(out error))
                {
                    if (EAutoResetEvent.WaitOne(TimeOut))
                    {
                        MReaderWriterLock.AcquireWriterLock(TimeOut);

                        if (_args.ParseOK)
                        {
                            MReaderWriterLock.ReleaseWriterLock();

                            return true;
                        }
                        error = _args.ErrorMsg;
                        MReaderWriterLock.ReleaseWriterLock();
                        return false;
                    }
                }
                return false;
            }
            catch(Exception exception)
            {
                error = exception.Message;
                MReaderWriterLock.ReleaseWriterLock();
                return false;
            }
           
        }
        


        public override CustomEventArgs RecvData()
        {
            if (!_haveReceData) return DAutoResetEvent.WaitOne(TimeOut) ? _args : null;
            _haveReceData = false;
            return _args;
        }


        public override void Handle(object sender, CustomEventArgs e)
        {       
            
            lock (Lockobject)
            {
                _args = e;
                if (_args.Mode.Equals(CallMode.AnsMode) || _args.Mode.Equals(CallMode.ErrMode))
                {
                    Debug.WriteLine("recv CallMode.AnsMode or ErrMode");
                    EAutoResetEvent.Set();
                }
                if (_args.Mode.Equals(CallMode.DataMode))
                {
                    Debug.WriteLine("recv CallMode.DataMode");
                    DAutoResetEvent.Set();
                    _haveReceData = true;

                }
            }
            
        }
    }

    public class ACNSerialLoaderCommand:SerialBaseComm
    {
        public ACNSerialLoaderCommand(SerialPort serialPort, string command)
        {
            _serialPort = serialPort;
            base.GetMsg(command);

        }


        public override bool Send(out string error)
        {
                return SendMsg(out error);
        }
        
       
    }


}
