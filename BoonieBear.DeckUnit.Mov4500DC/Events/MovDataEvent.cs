using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.ACMP;
using System.Collections;

namespace BoonieBear.DeckUnit.Mov4500UI.Events
{
    public class MovDataEvent
    {
        private ModuleType _type;
        private Hashtable _data;
        public MovDataEvent(ModuleType type, Hashtable ht)
        {
            _type = type;
            _data = ht;
        }

        public ModuleType Type
        {
            get { return _type; }
        }

        public Hashtable Data
        {
            get { return _data; }
        }
    }
}
