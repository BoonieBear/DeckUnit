using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BoonieBear.DeckUnit.CommLib.UDP
{
    public abstract class UDPBaseComm : Observer<CustomEventArgs>
    {
        protected UdpClient _udpClient;
        protected int BroadCastPort = 10020;
        public virtual bool Init(UdpClient udp)
        {

            _udpClient = udp;
            _udpClient.EnableBroadcast = true;
            return (_udpClient!=null);
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

        protected virtual bool SendTo(IPAddress ip, int port, byte[] bytes)
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
        protected virtual bool SendTo(IPAddress ip, int port, string cmd)
        {
            try
            {
                var sendBytes = Encoding.Default.GetBytes(cmd);
                SendTo(ip, port, sendBytes);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public abstract bool Send(IPAddress ip, int port);
        public abstract bool BroadCast();
        public void Handle(object sender, CustomEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
