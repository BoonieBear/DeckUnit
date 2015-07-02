using System.Windows;
using BoonieBear.DeckUnit.Mov4500UI.ViewModel;
using TinyMetroWpfLibrary.Controller;
using TinyMetroWpfLibrary.EventAggregation;
using BoonieBear.DeckUnit.Mov4500UI.Events;
namespace BoonieBear.DeckUnit.Mov4500UI.Core.Controllers
{
    /// <summary>
    /// 和页面导航相关消息处理函数，包括页面导航，导航传值，关闭页面，页面呈现
    /// BaseController已完成系统基础导航信息以及一些空间消息处理机制。
    /// </summary>
    internal class UnitNavigationController : BaseController,
        IHandleMessage<GoSystemResourceNavigationRequest>,
        IHandleMessage<GoLiveCaptureNavigationRequest>
    {

        public void Handle(GoSystemResourceNavigationRequest message)
        {
            NavigateToPage("Views/SystemResourceView.xaml");
        }
        public void Handle(GoLiveCaptureNavigationRequest message)
        {
            NavigateToPage("Views/LiveCaptureView.xaml");
        }
    }
}