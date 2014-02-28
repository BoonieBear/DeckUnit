using System.Windows;
using BoonieBear.DeckUnit.MessageEvents;
using BoonieBear.TinyMetro.WPF.Controller;
using BoonieBear.TinyMetro.WPF.EventAggregation;

namespace BoonieBear.DeckUnit.Core
{
    /// <summary>
    /// 消息处理函数
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