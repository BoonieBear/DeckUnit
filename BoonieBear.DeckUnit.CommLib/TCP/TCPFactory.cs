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
