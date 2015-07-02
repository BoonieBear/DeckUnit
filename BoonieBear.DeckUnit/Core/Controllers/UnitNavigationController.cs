using System.Windows;
using BoonieBear.DeckUnit.Events;
using TinyMetroWpfLibrary.Controller;
using TinyMetroWpfLibrary.EventAggregation;

namespace BoonieBear.DeckUnit.Core.Controllers
{
    /// <summary>
    /// 和页面导航相关消息处理函数，包括页面导航，导航传值，关闭页面
    /// BaseController已完成系统基础导航信息以及一些空间消息处理机制。
    /// </summary>
    internal class UnitNavigationController : BaseController,
        IHandleMessage<GoWaterTelPageBaseNavigationRequest>,
        IHandleMessage<GoHistoryDataPageBaseNavigationRequest>,
        IHandleMessage<GoSystemResourceNavigationRequest>,
        IHandleMessage<GoAcousticViewNavigationEvent>,
        IHandleMessage<GoHomePageNavigationEvent>

    {
        public void Handle(GoWaterTelPageBaseNavigationRequest message)
        {
            NavigateToPage("Views/WaterTelView.xaml",message.Titile);
            Application.Current.Properties["message"] = message.Titile;
        }

        public void Handle(GoHistoryDataPageBaseNavigationRequest message)
        {
            NavigateToPage("Views/HistoryDataPage.xaml", message.Titile);
            Application.Current.Properties["message"] = message.Titile;

        }

        public void Handle(GoSystemResourceNavigationRequest message)
        {
            NavigateToPage("Views/SystemResourceView.xaml");
        }
        public void Handle(GoAcousticViewNavigationEvent message)
        {
            NavigateToPage("Views/AcousticView.xaml");
        }
        public void Handle(GoHomePageNavigationEvent message)
        {
            NavigateToPage("Views/MainPageView.xaml");
        }
    }
}