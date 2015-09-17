using System.Net.Sockets;
using BoonieBear.DeckUnit.CommLib.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BoonieBear.DeckUnit.Comm.TCP
{
    public class ACMShellCommand : TCPBaseComm
    {
        public ACMShellCommand(TcpClient tcpClient, string cmd)
        {
            if (!base.Init(tcpClient)) return;
            cmd = cmd.TrimEnd('\n');
            base.GetMsg(cmd + "\r");
        }
        public override bool Send(out string error)
        {
            return SendMsg(out error);
        }
    }
    public class ACMDataCommand : TCPBaseComm
    {

        public ACMDataCommand(TcpClient tcpClient, byte[] bytes)
        {
            if (!base.Init(tcpClient)) return;
            if (bytes == null) return;
            base.LoadData(bytes);
        }

        public override bool Send(out string error)
        {
            return SendData(out error);
        }
    }
}

