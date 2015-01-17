using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BoonieBear.DeckUnit.CommLib.TCP
{
    public abstract class TCPBaseService:ITCPClientService
    {
        protected TcpClient _tcpClient;
        protected IPAddress IP;
        protected int port = 8080;
        public static event EventHandler<CustomEventArgs> DoParse;
        protected Thread TdThread;
        public TCPBaseService()
        {
            
        }


        public bool Init(TcpClient tcpClient, IPAddress ip,int destport)
        {
            if (tcpClient != null)
            {
                _tcpClient = tcpClient;
                IP = ip;
                port = destport;
                return true;
            }
                    return false;

         }

        public void Register(IObserver<CustomEventArgs> observer)
        {
            DoParse  -= observer.Handle;
            DoParse += observer.Handle;
        }
        public void UnRegister(IObserver<CustomEventArgs> observer)
        {
            DoParse -= observer.Handle;
        }
        public  void ConnectSync()
        {
            if (_tcpClient == null) return;
            try
            {
               // _tcpClient.Connect(IP, port);
                var result  = _tcpClient.BeginConnect(IP, port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                if (!_tcpClient.Connected)
                {
                    throw new Exception("连接失败");
                }
                // we have connected
                _tcpClient.EndConnect(result);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
            
        }

        public bool Start()
        {
            if (!_tcpClient.Connected) return false;
            TdThread = new Thread(RecvThread);
            TdThread.Start(_tcpClient);
            return true;
        }

        public abstract void RecvThread(object obj);

        public void Stop()
        {
            if (_tcpClient != null)
                _tcpClient.Close();
            if (TdThread != null)
            {
                if (TdThread.IsAlive)
                {
                    TdThread.Abort();

                }               
            }

        }

        public TcpClient ReturnTcpClient()
        {
            return _tcpClient;
        }

        public void OnParsed(CustomEventArgs eventArgs)
        {
            if (DoParse != null)
            {
                DoParse(this, eventArgs);
            }
        }

        public bool Connected { get { return ReturnTcpClient().Connected; } }
    }

    public class ACNTCPShellService:TCPBaseService
    {

        public override void RecvThread(object o)
        {
            TcpClient client = null;
            try
            {
                using (client = o as TcpClient)
                {
                    while (client.Connected)
                    {
                        var myCompleteMessage = new StringBuilder();
                        var myReadBuffer = new byte[2048];
                        var stream = client.GetStream();
                        do
                        {
                            var numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                            myCompleteMessage.AppendFormat("{0}",Encoding.Default.GetString(myReadBuffer, 0, numberOfBytesRead));
                        } while (stream.DataAvailable);
                        Debug.WriteLine(myCompleteMessage.ToString());
                        var e = new CustomEventArgs(myCompleteMessage.ToString(), myReadBuffer, 0, true, null,
                        CallMode.NoneMode, client);
                        OnParsed(e);
                    }
                   
                }
            }
            catch (Exception exception)
            {
                var e = new CustomEventArgs(null, null, 0, false, exception.Message, CallMode.ErrMode, client);
                OnParsed(e);
            }
            finally
            {
                if (client != null)
                    client.Close();
            }
        }
    }
    public class ACNTCPDataService : TCPBaseService
    {
        
        public override void RecvThread(object o)
        {
            TcpClient client = null;
            try
            {
                using (client = o as TcpClient)
                {
                    var myReadBuffer = new byte[4100];
                    var stream = client.GetStream();
                    while (stream.CanRead)
                    {
                        Array.Clear(myReadBuffer, 0, 4100);//置零
                        int numberOfBytesRead = 0;
                        stream.Read(myReadBuffer, 0, 4);//先读包头
                        var packetLength = BitConverter.ToUInt16(myReadBuffer, 2);
                        // Incoming message may be larger than the buffer size.
                        do
                        {
                            int n = stream.Read(myReadBuffer, 4 + numberOfBytesRead, packetLength - numberOfBytesRead);
                            numberOfBytesRead += n;

                        }
                        while (numberOfBytesRead != packetLength);
                        var e = new CustomEventArgs(null, myReadBuffer, packetLength, true, null, CallMode.DataMode, client);
                        OnParsed(e);
                    }
                }
            }
            catch (Exception exception)
            {
                var e = new CustomEventArgs(null, null, 0, false, exception.Message, CallMode.ErrMode, client);
                OnParsed(e);
            }
            finally
            {
                if(client!=null)
                    client.Close();
            }
            
        }

    }

    /// <summary>
    /// 通信机数据接收方法继承通行网的数据解析方法
    /// </summary>
    public class ACMDataService:ACNTCPDataService
    { }

    /// <summary>
    /// 甲板单元继承通信网的通信方式
    /// </summary>
    public class DeckUnitService:ACNTCPDataService
    { }
}
