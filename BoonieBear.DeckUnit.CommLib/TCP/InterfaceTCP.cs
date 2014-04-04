using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace BoonieBear.DeckUnit.CommLib.TCP
{
    

    public interface ITCPServiceFactory
    {
        ITCPClientService CreateService();
    }
    
}
