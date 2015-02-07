using BoonieBear.TinyMetro.WPF.Controller;

namespace BoonieBear.DeckUnit.ICore
{
    public interface IUnitKernel:IKernel
    {
            IMessageController MessageController { get; }
        
    }
}