using BoonieBear.TinyMetro.WPF.Controller;

namespace BoonieBear.DeckUnit.Core
{
    public interface IUnitKernel:IKernel
    {
            IMessageController MessageController { get; }
        
    }
}