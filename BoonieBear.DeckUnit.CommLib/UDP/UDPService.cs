using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BoonieBear.DeckUnit.CommLib.UDP
{
    public abstract class  UDPBaseService:IUDPService
    {
        protected UdpClient _udpClient;

        public static event EventHandler<CustomEventArgs> DoParse;
        private List<byte> _recvQueue = new List<byte>();
        public bool Init(UdpClient udpClient)
        {
            try
            {
                _recvQueue.Clear();
                _udpClient = udpClient;
                return (_udpClient.Client != null);
            }
            catch (Exception exception)
            {

                return false;
            }
        }

        

        public bool Start()
        {
            //打开udp监听端口
            var UdpReceiver = new Thread(ListensenUDP);
            UdpReceiver.Start();
            return UdpReceiver.IsAlive;
        }

        public void Stop()
        {
            if(_udpClient!=null)
                _udpClient.Close();
        }

        public UdpClient ReturnUdpClient()
        {
            return _udpClient;
        }
        public void Register(Observer<CustomEventArgs> observer)
        {
            DoParse -= observer.Handle;
            DoParse += observer.Handle;
        }
        public void UnRegister(Observer<CustomEventArgs> observer)
        {
            DoParse -= observer.Handle;
        }

        public void OnParsed(CustomEventArgs eventArgs)
        {
            if (DoParse != null)
            {
                DoParse(this, eventArgs);
            }
        }
        protected abstract void ListensenUDP();
    }
    //使用ACN协议的UDP DEBUG服务类，Start()中传入解析后的数据类
    public class ACNDebugUDPService:UDPBaseService
    {
        protected override void ListensenUDP()
        {
            var remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var flag = true;
            string returnData = string.Empty;
            string error = string.Empty;
            while (flag)
            {
                try
                {
                    var receiveBytes = _udpClient.Receive(ref remoteIpEndPoint);

                    returnData = Encoding.ASCII.GetString(receiveBytes);
                }
                catch (SocketException exception)
                {
                    if(exception.ErrorCode==0x2714)
                        break;
                    error = exception.Message;
                    flag = false;
                }
                finally
                {
                    var e = new CustomEventArgs(0, returnData, null, 0, flag, error, CallMode.NoneMode, _udpClient);
                    OnParsed(e);
                }
            }
        }
    }
    public class ACNDataUDPService:UDPBaseService
    {
        protected override void ListensenUDP()
        {
            var remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var flag =true;
            var buffer = new byte[4096];
            string error = string.Empty;
            var numberOfBytesRead = 0;
            var mode = CallMode.DataMode;
            while (flag)
            {
                try
                {
                    
                    Array.Clear(buffer, 0, 4096);
                    var receiveBytes = _udpClient.Receive(ref remoteIpEndPoint);
                    if (BitConverter.ToUInt16(receiveBytes, 0) != 0xEE01)
                        continue;
                    if (receiveBytes.Length < 4)
                        continue;
                    var PacketLength = BitConverter.ToUInt16(receiveBytes, 2);

                    Array.Copy(receiveBytes, 4, buffer, numberOfBytesRead, receiveBytes.Length - 4);
                    numberOfBytesRead = receiveBytes.Length - 4;
                    // Incoming message may be larger than the buffer size.
                    while (numberOfBytesRead < PacketLength)
                    {
                        receiveBytes = _udpClient.Receive(ref remoteIpEndPoint);
                        Array.Copy(receiveBytes, 0, buffer, numberOfBytesRead, receiveBytes.Length);
                        numberOfBytesRead += receiveBytes.Length;
                    }
                    
                }
                catch (SocketException exception)
                {
                    if (exception.ErrorCode != 0x2714) //程序关闭
                    {
                        error = exception.Message;
                        mode = CallMode.ErrMode;
                        flag = false;
                    }
                    else
                    {
                        flag = false;
                        return;
                    }
                
                }
                finally
                {
                    var e = new CustomEventArgs(0, string.Empty, buffer, numberOfBytesRead, flag, error, mode, _udpClient);
                    numberOfBytesRead = 0;
                    OnParsed(e);
                }
            }
        }
    }
    /// <summary>
    /// 4500接收外部广播数据服务1：避碰，ADCP，航控，
    /// </summary>
    public class ACMUWAService : UDPBaseService
    {
        protected override void ListensenUDP()
        {
            var remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var flag = true;
            byte[] buffer = null;
            string error = string.Empty;
            var mode = CallMode.Sail;
            while (flag)
            {
                try
                {
                    buffer = _udpClient.Receive(ref remoteIpEndPoint);

                }
                catch (SocketException exception)
                {
                    if (exception.ErrorCode != 0x2714) //程序关闭
                    {
                        error = exception.Message;
                        mode = CallMode.ErrMode;
                        flag = false;
                    }
                    else
                    {
                        flag = false;
                        return;
                    }

                }
                finally
                {
                    var e = new CustomEventArgs(0, string.Empty, buffer, buffer.Length, flag, error, mode, _udpClient);
                    OnParsed(e);
                }
            }
        }
    }
    /// <summary>
    /// 4500接收外部广播数据服务2：USBL
    /// </summary>
    public class ACMUSBLService : UDPBaseService
    {
        protected override void ListensenUDP()
        {
            var remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var flag = true;
            byte[] buffer = null;
            string error = string.Empty;
            var mode = CallMode.USBL;
            while (flag)
            {
                try
                {
                    buffer = _udpClient.Receive(ref remoteIpEndPoint);
                }
                catch (SocketException exception)
                {
                    if (exception.ErrorCode != 0x2714) //程序关闭
                    {
                        error = exception.Message;
                        mode = CallMode.ErrMode;
                        flag = false;
                    }
                    else
                    {
                        flag = false;
                        return;
                    }

                }
                finally
                {
                    var e = new CustomEventArgs(0, string.Empty, buffer, buffer.Length, flag, error, mode, _udpClient);
                    OnParsed(e);
                }
            }
        }
    }
    /// <summary>
    /// 4500接收外部广播数据服务2：GPS
    /// </summary>
    public class ACMGPSService : UDPBaseService
    {
        protected override void ListensenUDP()
        {
            var remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var flag = true;
            byte[] buffer = null;
            string error = string.Empty;
            var mode = CallMode.GPS;
            while (flag)
            {
                try
                {
                    buffer = _udpClient.Receive(ref remoteIpEndPoint);
                }
                catch (SocketException exception)
                {
                    if (exception.ErrorCode != 0x2714) //程序关闭
                    {
                        error = exception.Message;
                        mode = CallMode.ErrMode;
                        flag = false;
                    }
                    else
                    {
                        flag = false;
                        return;
                    }

                }
                finally
                {
                    var e = new CustomEventArgs(0, string.Empty, buffer, buffer.Length, flag, error, mode, _udpClient);
                    OnParsed(e);
                }
            }
        }
    }
}
