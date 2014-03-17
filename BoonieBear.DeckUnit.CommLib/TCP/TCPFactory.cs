using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BoonieBear.DeckUnit.CommLib.TCP
{
    /// <summary>
    /// TCP命令工厂
    /// </summary>
    public class TCPCommFactory : ITCPCommFactory
    {
        private TcpClient _tcpClient;
        public TCPCommFactory(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }
        public TCPBaseComm CreateTCPComm(ACNCommandMode mode, byte[] bytes,string shellcmd,Stream stream)
        {
            switch (mode)
            {
                case ACNCommandMode.CmdCharMode:
                    return new ACNTCPShellCommand(_tcpClient,shellcmd);
                    break;
                case ACNCommandMode.CmdWithData:
                    return new ACNTCPDataCommand(_tcpClient,bytes);
                    break;
                case ACNCommandMode.CmdWithStream:
                    return new ACNTCPStreamCommand(_tcpClient,stream);
                    break;
                default:
                    throw new InvalidOperationException("不支持的命令模式！");
                    break;
            }
        }
    }
    /// <summary>
    /// TCP Shell服务
    /// </summary>
    public class TCPShellServiceFactory : ITCPServiceFactory
    {
        public ITCPClientService CreateService()
        {
            return new TCPShellService();
        }
    }
    /// <summary>
    /// TCP Shell服务
    /// </summary>
    public class TCPDataServiceFactory : ITCPServiceFactory
    {
        public ITCPClientService CreateService()
        {
            return new TCPDataService();
        }
    }
}
