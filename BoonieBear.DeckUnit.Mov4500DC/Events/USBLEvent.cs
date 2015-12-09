using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.ACMP;

namespace BoonieBear.DeckUnit.Mov4500UI.Events
{
    public class USBLEvent
    {
        public Sysposition Position { get; private set; }

        public USBLEvent(Sysposition pos)
        {
            Position = pos;
        }
    }
}
