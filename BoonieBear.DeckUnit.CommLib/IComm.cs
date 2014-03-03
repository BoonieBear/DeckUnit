using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BoonieBear.DeckUnit.CommLib
{
    //命令类接口，用在Command类中，使用各个命令类的实现完成各个命令的发送
    public interface IComm
    {
        bool Send();
    }
  


     
}
