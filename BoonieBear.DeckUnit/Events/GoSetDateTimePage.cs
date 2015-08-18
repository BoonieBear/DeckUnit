using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoonieBear.DeckUnit.Events
{
    class GoSetDateTimePage
    {
        private string _titile;
        public GoSetDateTimePage(string titile)
		{
		    _titile = titile;
		}

        public string Titile
        {
            get { return _titile; }
        }
    }
}
