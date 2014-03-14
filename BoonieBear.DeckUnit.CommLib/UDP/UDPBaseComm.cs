using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BoonieBear.DeckUnit.CommLib.UDP
{
    public abstract class UDPBaseComm : IObserver<CustomEventArgs>
    {
        protected UdpClient _udpClient;
        protected int BroadCastPort = 10020;
        public virtual bool Init(UdpClient udp)
        {
            _udpClient = udp;
            return (_udpClient!=null)&&(_udpClient.EnableBroadcast);
        }
        protected virtual bool BroadCast(byte[] bytes)
        {
            return SendTo(IPAddress.Broadcast, BroadCastPort, bytes);
        }

        protected virtual bool BroadCastMsg(string str)
        {
            var sendBytes = Encoding.Default.GetBytes(str);
            return BroadCast(sendBytes);
        }

        protected virtual bool SendTo(IPAddress ip,int port,byte[] bytes)
        {
            try
            {
                _udpClient.Send(bytes, bytes.Length, ip.ToString(), port);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public void Handle(object sender, CustomEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
