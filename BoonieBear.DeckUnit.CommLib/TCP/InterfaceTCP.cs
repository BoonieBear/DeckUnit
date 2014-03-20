using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace BoonieBear.DeckUnit.CommLib.TCP
{
    #region TCP命令工厂接口和服务接口

    public interface ITCPCommFactory
    {
        TCPBaseComm CreateTCPComm(ACNCommandMode mode,  byte[] bytes,string shellcmd,Stream stream);
    }

    public interface ITCPServiceFactory
    {
        ITCPClientService CreateService();
    }
    #endregion 
}
