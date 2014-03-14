using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BoonieBear.DeckUnit.CommLib.UDP
{
    public class UDPCommFactory:IUDPCommFactory
    {
        private UdpClient _udpClient;

        public UDPCommFactory(UdpClient udpClient)
        {
            _udpClient = udpClient;
        }
        public UDPBaseComm CreateSerialComm(ACNCommandMode mode, int id, byte[] bytes,string str)
        {
            return new ACNUDPCommand(_udpClient, mode, str, bytes);
        }
    }
    public class UDPDebugServiceFactory:IUDPServiceFactory
    {
        public IUDPService CreateService()
        {
            return new ACNDebugUDPService();
        }
    }
    public class UDPDataServiceFactory : IUDPServiceFactory
    {
        public IUDPService CreateService()
        {
            return new ACNDataUDPService();
        }
    }
}
