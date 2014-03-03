using System.Windows;
using BoonieBear.DeckUnit.MessageEvents;
using BoonieBear.TinyMetro.WPF.Controller;
using BoonieBear.TinyMetro.WPF.EventAggregation;

namespace BoonieBear.DeckUnit.Core
{
    /// <summary>
    /// 和页面导航相关消息处理函数，包括页面导航，导航传值，关闭页面
    /// </summary>
    internal class UnitNavigationController : BaseController,
        IHandleMessage<GoWaterTelPageBaseNavigationRequest>,
        IHandleMessage<GoHistoryDataPageBaseNavigationRequest>

    {
        public void Handle(GoWaterTelPageBaseNavigationRequest message)
        {
            NavigateToPage("Views/WaterTelView.xaml");
        }

        public void Handle(GoHistoryDataPageBaseNavigationRequest message)
        {
            NavigateToPage("Views/HistoryDataPage.xaml", message.Titile);
            Application.Current.Properties["message"] = message.Titile;

        }
    }
}