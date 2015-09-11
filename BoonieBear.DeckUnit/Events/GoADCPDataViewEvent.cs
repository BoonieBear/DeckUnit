using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.BaseType;
using BoonieBear.DeckUnit.UBP;
namespace BoonieBear.DeckUnit.Events
{
    
    class GoADCPDataViewEvent
    {
        public BDTask CurrentBdTask { get; set; }
        public GoADCPDataViewEvent(BDTask bdTask)
        {
            CurrentBdTask = bdTask;
        }
    }
    
}
