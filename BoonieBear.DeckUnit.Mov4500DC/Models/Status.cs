using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.CommLib.Properties;
using BoonieBear.DeckUnit.Mov4500UI.Core;
using BoonieBear.DeckUnit.Mov4500UI.Helpers;
namespace BoonieBear.DeckUnit.Mov4500UI.Models
{
    public class Status
    {

        public static string NetworkStatus
        {
            get
            {
                if (UnitCore.Instance.NetCore.IsTCPWorking)
                    return Properties.Resources.NETWORK_OK;
                return Properties.Resources.NETWORK_DOWN;
            }
        }
        public static string CommStatus
        {
            get
            {
                if (UnitCore.Instance.CommCore.IsWorking)
                    return Properties.Resources.Comm_OK;
                return Properties.Resources.Comm_Failed;
            }
        }
 

        public static string LastUpdateTime { get; set; }

        public static UInt16 ReceiveMsgCount { get; set; }

    }
}
