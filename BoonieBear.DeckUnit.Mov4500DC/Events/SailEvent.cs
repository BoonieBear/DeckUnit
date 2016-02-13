using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.ACMP;
namespace BoonieBear.DeckUnit.Mov4500UI.Events
{

    public class SailEvent
    {
        private ExchageType _type;
        private IProtocol _data;
        public SailEvent(ExchageType type, IProtocol sailData)
        {
            _type = type;
            _data = sailData;
        }

        public ExchageType Type
        {
            get { return _type; }
        }

        public IProtocol SailData
        {
            get { return _data; }
        }
    }
}
