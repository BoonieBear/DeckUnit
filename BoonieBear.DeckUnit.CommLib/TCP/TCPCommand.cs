using System;
using System.IO;
using System.Net.Sockets;
using BoonieBear.DeckUnit.CommLib.TCP;

namespace BoonieBear.DeckUnit.CommLib
{
    public class ACNTCPShellCommand:TCPBaseComm
    {
        private string _cmd;

        public ACNTCPShellCommand(TcpClient tcpClient,string cmd)
        {
            if (!base.Init(tcpClient)) return;
            _cmd = cmd;
        }

        public override bool Send(out string error)
        {
            throw new NotImplementedException();
        }
    }

    public class ACNTCPDataCommand : TCPBaseComm
    {
        private byte[] _bytes;

        public ACNTCPDataCommand(TcpClient tcpClient, byte[] bytes)
        {
            if (!base.Init(tcpClient)) return; 


            _bytes = new byte[bytes.Length];
            Array.Copy(bytes, _bytes, bytes.Length);
        }

        public override bool Send(out string error)
        {
            throw new NotImplementedException();
        }
    }
    public class ACNTCPStreamCommand : TCPBaseComm
    {
        private Stream _filestream =null;
        public ACNTCPStreamCommand(TcpClient tcpClient,Stream stream)
        {
            if (!base.Init(tcpClient)) return;
            _filestream = stream;
        }

        public override bool Send(out string error)
        {
            throw new NotImplementedException();
        }
    }
}
