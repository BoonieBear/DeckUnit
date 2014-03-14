using System;
using System.Net.Sockets;

namespace BoonieBear.DeckUnit.CommLib.UDP
{
    public class ACNUDPCommand:UDPBaseComm
    {
        ACNCommandMode _mode = ACNCommandMode.CmdCharMode;
        private string cmd = null;
        private byte[] _bytes;
        public ACNUDPCommand(UdpClient udpClient, ACNCommandMode mode,string str,byte[] bytes)
        {
            _udpClient = udpClient;
            _mode = mode;
            if (mode == ACNCommandMode.CmdCharMode)
            {
                cmd = str;
            }
            if (mode == ACNCommandMode.CmdWithData)
            {
                _bytes =new byte[bytes.Length];
                Array.Copy(bytes,_bytes,bytes.Length);
            }

        }

    }
}
