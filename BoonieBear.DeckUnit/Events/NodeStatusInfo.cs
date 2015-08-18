using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoonieBear.DeckUnit.Events
{
    public class NodeStatusInfo
    {
        public NodeStatusInfo(string str, byte[] data)
        {
            info = str;
            Data = data;
        }
        public byte[] Data { get; set; }
        public string info { get; set; }
    }
}
