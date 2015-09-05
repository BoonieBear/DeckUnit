using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.UBP;
namespace BoonieBear.DeckUnit.Events
{
    
    class GoADCPDataViewEvent
    {
        public Task CurrentTask { get; set; }
        public GoADCPDataViewEvent(Task task)
        {
            CurrentTask = task;
        }
    }
    
}
