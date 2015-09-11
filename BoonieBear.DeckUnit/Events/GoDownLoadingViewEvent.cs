using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.BaseType;
namespace BoonieBear.DeckUnit.Events
{
    
    public class GoDownLoadingViewEvent
    {
        private BDTask _passBdTask ;

        public GoDownLoadingViewEvent(BDTask bdTask)
        {
            _passBdTask = bdTask;
        }

        public BDTask NewBdTask
        {
            get { return _passBdTask; }
        }
    }
}
