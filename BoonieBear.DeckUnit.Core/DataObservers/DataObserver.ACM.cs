using System;
using BoonieBear.DeckUnit.CommLib;

namespace BoonieBear.DeckUnit.Core.DataObservers
{
    public class ACMDataObserver : CommLib.IObserver<CustomEventArgs>
    {
        public void Handle(object sender, CustomEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
