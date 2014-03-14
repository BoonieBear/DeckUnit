using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BoonieBear.DeckUnit.CommLib.Serial;

namespace BoonieBear.DeckUnit.CommLib
{
    /// <summary>
    /// 命令模式的命令类实现，调用IComm
    /// </summary>
    public  class Command
    {
        public static string Error;

        public static Task<bool> SendCommandAsync(SerialBaseComm command)
        {
            return Task.Factory.StartNew(() => command.Send(out Error));
           
        }
        public static Task<CustomEventArgs> RecvDataAsync(SerialBaseComm command)
        {
            return Task.Factory.StartNew(() => command.RecvData());

        }

    }

}
