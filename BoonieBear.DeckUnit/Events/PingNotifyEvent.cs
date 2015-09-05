using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoonieBear.DeckUnit.Events
{
    public class PingNotifyEvent
    {
        public PingNotifyEvent(string str)
        {
            info = str;
        }
        public string info { get; set; }
    }
}
