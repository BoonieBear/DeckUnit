using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.ACMP;
using BoonieBear.DeckUnit.Mov4500UI.Core;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;
namespace BoonieBear.DeckUnit.Mov4500UI.ViewModel
{
    public class LiveCaptureViewModel : ViewModelBase
    {
        public override void Initialize()
        {
        }
        public override void InitializePage(object extraData)
        {
            BtnShow = (UnitCore.Instance.WorkMode == MonitorMode.SHIP) ? true : false;
        }

        //show btn or control based on the workmode 
        //if workmode == ship tnshow = true, or btnshow=false
        public bool BtnShow
        {
            get { return GetPropertyValue(() => BtnShow); }
            set { SetPropertyValue(() => BtnShow, value); }
        }
    }
}
