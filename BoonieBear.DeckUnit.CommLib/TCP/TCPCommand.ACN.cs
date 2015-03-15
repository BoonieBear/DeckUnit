using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using TinyMetroWpfLibrary.Utility;

namespace BoonieBear.DeckUnit.CommLib.TCP
{
    public class ACNTCPShellCommand:TCPBaseComm
    {

        public ACNTCPShellCommand(TcpClient tcpClient,string cmd)
        {
            if (!base.Init(tcpClient)) return;
            base.GetMsg(cmd+"\r");
        }

        public override bool Send(out string error)
        {
            
            return SendMsg(out error);
  
        }
    }

    public class ACNTCPDataCommand : TCPBaseComm
    {

        public ACNTCPDataCommand(TcpClient tcpClient, byte[] bytes)
        {
            if (!base.Init(tcpClient)) return;
            if (bytes==null) return;
            base.GetData(bytes);
        }

        public override bool Send(out string error)
        {
            return SendData(out error);
        }
    }
    //异步下载大数据，需要回传进度
    public class ACNTCPStreamCommand : TCPBaseComm
    {
        private Stream _filestream;
        private CustomEventArgs _args = new CustomEventArgs(null, null, 0, false, null, CallMode.NoneMode,null);
        private static readonly AutoResetEvent EAutoResetEvent = new AutoResetEvent(false);
        private delegate void ReportProgressEvent(int i);
        private event ReportProgressEvent ReportProgress;
        private const int TimeOut = 20000;
        private static readonly object Lockobject = new object();
        public ACNTCPStreamCommand(TcpClient tcpClient,Stream stream,Action<int> progressAction)
        {
            if (!base.Init(tcpClient)) return;
            _filestream = stream;
            ReportProgress = new ReportProgressEvent(progressAction);
        }

        public override bool Send(out string error)
        {

            bool isEnd = true;
            error = null;
            try
            {
                var filereader = new BinaryReader(_filestream);
                var mySendBuffer = new byte[1028];
                var sendBytes = 0;
                Int16 numberOfBytesRead = 0;
                const ushort head = 0xDADA;
                Buffer.BlockCopy(BitConverter.GetBytes(head), 0, mySendBuffer, 0, 2);
                while ((numberOfBytesRead = (Int16)filereader.Read(mySendBuffer, 4, 1024)) != 0)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(numberOfBytesRead), 0, mySendBuffer, 2, 2);
                    base.GetData(mySendBuffer);
                    
                    //ReportProgress(sendBytes);
                    if (SendData(out error))
                    {
                        if (EAutoResetEvent.WaitOne(TimeOut))
                        {
                            Debug.WriteLine("WaitOne!!!");

                            if (!_args.ParseOK)
                            {
                                 error = _args.ErrorMsg;
                                isEnd = false;
                                break;
                            }
                            sendBytes += numberOfBytesRead;
                        }
                        else
                        {
                            error = " 接收数据超时！";
                            isEnd = false;
                            break;
                        }
                    }
                    else
                    {
                        
                        isEnd = false;
                        break;
                    } 
                }
                //循环接收，两种可能：超时or出错；发完了
                if (isEnd)
                {
                    filereader.BaseStream.Seek(0, SeekOrigin.Begin);//回到文件头
                    var totalb = filereader.ReadBytes((int)filereader.BaseStream.Length);
                    var crc = CRCHelper.CRC16Byte(totalb);
                    filereader.Close();
                    const int End = 0xEDED;
                    Buffer.BlockCopy(BitConverter.GetBytes(End), 0, mySendBuffer, 0, 2);
                    Buffer.BlockCopy(BitConverter.GetBytes(crc), 0, mySendBuffer, 2, 2);
                    base.GetData(mySendBuffer);
                    isEnd = SendData(out error);
                }
               
                return isEnd;
            }
            catch(Exception exception)
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
                if ((_args.Mode.Equals(CallMode.DataMode)) && (BitConverter.ToUInt16(_args.DataBuffer, 0) == 0xACAC))
                {
                   
                   EAutoResetEvent.Set();
                    
                }
            }
        }
    }
}
