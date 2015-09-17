using System;
using System.Net;
using System.Net.Sockets;

namespace BoonieBear.DeckUnit.CommLib.UDP
{
    public class ACNUDPShellCommand:UDPBaseComm
    {

        private string cmd = null;
        public ACNUDPShellCommand(UdpClient udpClient,string str)
        {
            if (!base.Init(udpClient)) return;

            cmd = str;

        }

        public override bool Send(IPAddress ip, int port)
        {
 
            return SendTo(ip,port,cmd);

        }

        public override bool BroadCast()
        {
            
              return BroadCastMsg(cmd);
           
        }
    }
    public class ACNUDPDataCommand : UDPBaseComm
    {
        
        private byte[] _bytes;

        public ACNUDPDataCommand(UdpClient udpClient, byte[] bytes)
        {
            if (!base.Init(udpClient)) return;
            _bytes = new byte[bytes.Length];
            Array.Copy(bytes, _bytes, bytes.Length);
        }

        public override bool Send(IPAddress ip, int port)
        {
           
            return SendTo(ip, port, _bytes);
           
        }

        public override bool BroadCast()
        {

           return BroadCast(_bytes);
           
        }
    }
    public class ACMDataBroadCast : UDPBaseComm
    {
        private byte[] _bytes;

        public ACMDataBroadCast(UdpClient udpClient, byte[] bytes)
        {
            if (!base.Init(udpClient)) return;
            _bytes = new byte[bytes.Length];
            Array.Copy(bytes, _bytes, bytes.Length);
        }

        public override bool Send(IPAddress ip, int port)
        {
           
            return SendTo(ip, port, _bytes);
           
        }

        public override bool BroadCast()
        {

           return BroadCast(_bytes);
           
        }
    }
}
