using BoonieBear.DockUnit.NavigationEvents;
using BoonieBear.TinyMetro.WPF.Controller;
using BoonieBear.TinyMetro.WPF.EventAggregation;

namespace BoonieBear.DockUnit.Core
{
    /// <summary>
    /// 消息处理函数
    /// </summary>
    internal class TestController : BaseController,
        IHandleMessage<GoPage1NavigationRequest>
    {
        public void Handle(GoPage1NavigationRequest message)
        {
            NavigateToPage("Views/Page1View.xaml");
        }
    }
}