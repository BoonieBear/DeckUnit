using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace BoonieBear.DeckUnit.CommLib
{
    /// <summary>
    /// 命令模式的命令类实现，调用ISerialComm和ITCPComm
    /// </summary>
    public  class Command
    {
        public bool SendSerialCommand(IComm command)
        {
            return command.Send();
        }
     
    }

}
