using BoonieBear.DockUnit.NavigationEvents;
using BoonieBear.TinyMetro.WPF.Controller;
using BoonieBear.TinyMetro.WPF.EventAggregation;

namespace BoonieBear.DockUnit.Core
{
    /// <summary>
    /// 消息处理函数
    /// </summary>
    internal class UnitController : BaseController,
        IHandleMessage<GoWaterTelPageNavigationRequest>
    {
        public void Handle(GoWaterTelPageNavigationRequest message)
        {
            NavigateToPage("Views/WaterTelView.xaml");
        }
    }
}