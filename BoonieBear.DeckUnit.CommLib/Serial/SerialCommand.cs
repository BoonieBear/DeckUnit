using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BoonieBear.DeckUnit.CommLib.Protocol;

namespace BoonieBear.DeckUnit.CommLib.Serial
{
    public class ACNSerialHexCommand : SerialBaseComm
    {
        private  CustomEventArgs _args = new CustomEventArgs(null,null,0,false,null,CallMode.NoneMode);
        private static readonly ReaderWriterLock MReaderWriterLock = new ReaderWriterLock();
        private static readonly AutoResetEvent EAutoResetEvent = new AutoResetEvent(false);
        private static readonly AutoResetEvent DAutoResetEvent = new AutoResetEvent(false);
        private static bool _haveReceData = false;
        private const int TimeOut = 20000;
        private static readonly object Lockobject = new object();
        public ACNSerialHexCommand(SerialPort serialPort,ACNCommandMode mode,int id,byte[] bytes)
        {
            _serialPort = serialPort;
            switch (mode)
            {
                case ACNCommandMode.CmdIDMode:
                    var cmd = new byte[1];
                    cmd[0] =Convert.ToByte(id);
                    _nBytes = ACNProtocol.CommPackage(id, cmd);
                    break;
                case ACNCommandMode.CmdWithData:
                    _nBytes = ACNProtocol.CommPackage(id, bytes);
                    break;
                default:
                    _nBytes = ACNProtocol.CommPackage(id, bytes);
                    break;
            }
        }
  
        public override bool Send(out string error)
        {
  
            //发送成功
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
                    MReaderWriterLock.ReleaseWriterLock();
                    
                    return false;
                }

                
                return false;
            }
            
            return false;
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

                    EAutoResetEvent.Set();
                }
                if (_args.Mode.Equals(CallMode.DataMode))
                {
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
                    break;
                case ACNCommandMode.LoaderDataMode:
                    return SendData(out error);
                    break;
                default:
                    return SendMsg(out error);
                    break;
            }
            
        }
        
       
    }


}
