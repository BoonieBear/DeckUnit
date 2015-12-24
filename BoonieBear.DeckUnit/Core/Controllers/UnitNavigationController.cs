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
        IHandleMessage<GoHistoryDataPageBaseNavigationRequest>,
        IHandleMessage<GoAcousticViewNavigationEvent>,
        IHandleMessage<GoHomePageNavigationEvent>,
        IHandleMessage<GoSimplePage>,
        IHandleMessage<GoDeviceBackSetPage>,
        IHandleMessage<GoDeviceParaSetPage>,
        IHandleMessage<GoNodeRecvEmitSetPage>,
        IHandleMessage<GoComSchemaEvent>,
        IHandleMessage<GoSetDateTimePage>,
        IHandleMessage<GoDebugViewPage>,
        IHandleMessage<GoCofigViewPage>,
        IHandleMessage<GoConnectConfigViewEvent>,
        IHandleMessage<GoSetEnergyViewEvent>,
        IHandleMessage<GoPingTestViewEvent>,
        IHandleMessage<GoGetNodeStatusViewEvent>,
        IHandleMessage<GoRefreshNodeConfigViewEvent>,
        IHandleMessage<GoDownLoadingViewEvent>,
        IHandleMessage<GoNewTaskViewEvent>,
        IHandleMessage<GoADCPDataViewEvent>,
        IHandleMessage<GoTaskListViewEvent>,
        IHandleMessage<GoADViewEvent>
    {

        public void Handle(GoHistoryDataPageBaseNavigationRequest message)
        {
            NavigateToPage("Views/HistoryDataPage.xaml");

        }

        
        public void Handle(GoAcousticViewNavigationEvent message)
        {
            NavigateToPage("Views/AcousticView.xaml");
        }

        public void Handle(GoHomePageNavigationEvent message)
        {
            NavigateToPage("Views/MainPageView.xaml");
        }

        public void Handle(GoSimplePage message)
        {
            NavigateToPage("Views/CommandView/SimpleView.xaml",message.Titile);
        }
        public void Handle(GoDeviceBackSetPage message)
        {
            NavigateToPage("Views/CommandView/DeviceBackSetView.xaml");
        }
        public void Handle(GoDeviceParaSetPage message)
        {
            NavigateToPage("Views/CommandView/DeviceParaSetView.xaml");
        }

        public void Handle(GoNodeRecvEmitSetPage message)
        {
            NavigateToPage("Views/CommandView/NodeRecvEmitSetView.xaml");
        }

        public void Handle(GoComSchemaEvent message)
        {
            NavigateToPage("Views/CommandView/NodeComSchema.xaml");
        }
        public void Handle(GoSetDateTimePage message)
        {
            NavigateToPage("Views/SetView/SetDateTimeView.xaml", message.Titile);
        }
        public void Handle(GoDebugViewPage message)
        {
            NavigateToPage("Views/SetView/DebugView.xaml");
        }
        public void Handle(GoCofigViewPage message)
        {
            NavigateToPage("Views/SetView/ConfigView.xaml");
        }

        public void Handle(GoConnectConfigViewEvent message)
        {
            NavigateToPage("Views/SetView/ConnectConfigView.xaml");
        }

        public void Handle(GoSetEnergyViewEvent message)
        {
            NavigateToPage("Views/SetView/SetEnergyInfoView.xaml");
        }
        public void Handle(GoPingTestViewEvent message)
        {
            NavigateToPage("Views/SetView/PingTestView.xaml");
        }
        public void Handle(GoGetNodeStatusViewEvent message)
        {
            NavigateToPage("Views/SetView/GetNodeStatusView.xaml");
        }
        public void Handle(GoRefreshNodeConfigViewEvent message)
        {
            NavigateToPage("Views/SetView/RefreshNodeConfigView.xaml");
        }

        public void Handle(GoDownLoadingViewEvent message)
        {
            NavigateToPage("Views/DownLoadingView.xaml",message);
        }

        public void Handle(GoADCPDataViewEvent message)
        {
            NavigateToPage("Views/ADCPDataView.xaml", message);
        }

        public void Handle(GoNewTaskViewEvent message)
        {
            NavigateToPage("Views/NewTaskPage.xaml");
        }
        public void Handle(GoTaskListViewEvent message)
        {
            //NavigateToPage("Views/DownLoadedView.xaml");
        }

        public void Handle(GoADViewEvent message)
        {
            NavigateToPage("Views/ADPage.xaml");
        }
    }
}