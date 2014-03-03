using System.Net.Sockets;

namespace BoonieBear.DeckUnit.CommLib.TCP
{
    public abstract class TCPBaseComm : ITCPComm
    {

        public bool Send()
        {
            throw new System.NotImplementedException();
        }

        public bool SendMsg()
        {
            throw new System.NotImplementedException();
        }

        public bool Init(TcpClient tcpClient)
        {
            throw new System.NotImplementedException();
        }

        public void GetMsg(string str)
        {
            throw new System.NotImplementedException();
        }

        public void GetData(byte[] bytes)
        {
            throw new System.NotImplementedException();
        }
    }
}
