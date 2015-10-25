using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoonieBear.DeckUnit.Events
{
    public class UpdateADByteCount
    {
        private int ADCount = 0;
        public UpdateADByteCount(int nCount)
        {
            AdCount = nCount;
        }

        public int AdCount
        {
            get { return ADCount; }
            set { ADCount = value; }
        }
    }
}
