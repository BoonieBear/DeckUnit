using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using BoonieBear.DeckUnit.Protocol.ACNSeries;

namespace BoonieBear.DeckUnit.CommLib.Serial
{
    public class ACNSerialHexCommand : SerialBaseComm
    {
        private  CustomEventArgs _args = new CustomEventArgs(null,null,0,false,null,CallMode.NoneMode,null);
        private static readonly ReaderWriterLock MReaderWriterLock = new ReaderWriterLock();
        private static readonly AutoResetEvent EAutoResetEvent = new AutoResetEvent(false);
        private static readonly AutoResetEvent DAutoResetEvent = new AutoResetEvent(false);
        private static bool _haveReceData = false;
        private const int TimeOut = 2000;
        private static readonly object Lockobject = new object();
        public ACNSerialHexCommand(SerialPort serialPort,int id,byte[] bytes)
        {
            _serialPort = serialPort;
           
            _nBytes = ACNProtocol.CommPackage(id, bytes);

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
                if (_args.Mode.Equals(CallMode.AnsMode))
                {
                    Debug.WriteLine("recv CallMode.AnsMode");
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
        ACNCommandMode _mode = ACNCommandMode.CmdCharMode;

        public ACNSerialLoaderCommand(SerialPort serialPort, ACNCommandMode mode, string command, byte[] bytes)
        {
            _serialPort = serialPort;
            _mode = mode;
            switch (mode)
            {
                case ACNCommandMode.CmdCharMode:
                    base.GetMsg(command);
                    break;
                case ACNCommandMode.LoaderDataMode:
                    _nBytes = new byte[bytes.Length];
                    Array.Copy(bytes, _nBytes, bytes.Length);
                    break;
                default:
                    break;
            }


        }


        public override bool Send(out string error)
        {
            switch (_mode)
            {
                case ACNCommandMode.CmdCharMode:
                    return SendMsg(out error);
                    
                case ACNCommandMode.LoaderDataMode:
                    return SendData(out error);
                    
                default:
                    return SendMsg(out error);
                    
            }
            
        }
        
       
    }


}
