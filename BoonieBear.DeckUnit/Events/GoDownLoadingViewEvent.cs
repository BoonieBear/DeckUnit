using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.DAL;

namespace BoonieBear.DeckUnit.Events
{
    
    public class GoDownLoadingViewEvent
    {
        private Task _passTask ;

        public GoDownLoadingViewEvent(Task task)
        {
            _passTask = task;
        }

        public Task NewTask
        {
            get { return _passTask; }
        }
    }
}
