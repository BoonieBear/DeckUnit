
using BoonieBear.TinyMetro.WPF.Controller;
using BoonieBear.TinyMetro.WPF.EventAggregation;

namespace BoonieBear.DeckUnit.Core
{
    /// <summary>
    /// 模块间消息处理函数，包括WriteLine，数据传递，系统消息广播，报警等
    /// </summary>
    class UnitMessageController
    {
        protected UnitMessageController()
        {
            IEventAggregator eventAggregator = Kernel.Instance.EventAggregator;
            //将类实例注册到EventAggregator
            eventAggregator.Subscribe(this);
        }

    }
}
