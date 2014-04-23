using BoonieBear.DeckUnit.Core.Controllers;
using BoonieBear.TinyMetro.WPF.Controller;
using BoonieBear.TinyMetro.WPF.EventAggregation;

namespace BoonieBear.DeckUnit.Core
{
    public interface IUnitKernel:IKernel
    {
            IMessageController MessageController { get; }
        
    }
}