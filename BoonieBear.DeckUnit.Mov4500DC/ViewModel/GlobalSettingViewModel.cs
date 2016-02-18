using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BoonieBear.DeckUnit.ACMP;
using BoonieBear.DeckUnit.Mov4500UI.Core;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using MahApps.Metro.Controls.Dialogs;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;
using System.Net;
using BoonieBear.DeckUnit.Mov4500UI.Helpers;
namespace BoonieBear.DeckUnit.Mov4500UI.ViewModel
{
    public class GlobalSettingViewModel : ViewModelBase
    {
        private bool bInitial = false;
        public override void Initialize()
        {
            FetchingVersion = RegisterCommand(ExecuteFetchingVersion, CanExecuteFetchingVersion, true);
            LinkCheckCommand = RegisterCommand(ExecuteLinkCheckCommand, CanExecuteLinkCheckCommand, true);
            LinkUnCheckCommand = RegisterCommand(ExecuteLinkUnCheckCommand, CanExecuteLinkUnCheckCommand, true);
            SaveConfig = RegisterCommand(ExecuteSaveConfig, CanExecuteSaveConfig, true);
            IsFetching = false;
            Version = "0.0.0"; 
            RefreshVisble = Visibility.Visible;
            IsUpdating = false;
            UpdatePercentange = 0;
            XmtIndex = 0;
            XMTValue = 0.1F;
            Gain = 39;
        }
        public override void InitializePage(object extraData)
        {
            bInitial = false;
            if(!UnitCore.Instance.LoadConfiguration())
                return;
            var conf = UnitCore.Instance.MovConfigueService.GetCommConfInfo();
            SelectMode = (int) UnitCore.Instance.WorkMode;
            if (conf != null)
            {
                ShipIpAddr = UnitCore.Instance.MovConfigueService.GetShipIP();
                UWVIpAddr = UnitCore.Instance.MovConfigueService.GetUWVIP();
                XmtIndex = int.Parse(UnitCore.Instance.MovConfigueService.GetXmtChannel()) - 1;
                XMTValue = float.Parse(UnitCore.Instance.MovConfigueService.GetXmtAmp());
                Gain = int.Parse(UnitCore.Instance.MovConfigueService.GetGain());
            }
            if (UnitCore.Instance.NetCore.IsTCPWorking)
            {
                
                
                if (SelectMode==0&&ShipIpAddr == conf.LinkIP)
                {
                    ShipConnected = true;
                    UWVConnected = false;
                }
                else if(SelectMode==1&&UWVIpAddr==conf.LinkIP)
                {
                    UWVConnected = true;
                    ShipConnected = false;
                }
                TaskEx.Delay(100);
                ExecuteFetchingVersion(null, null);
            }
            else
            {
                ShipConnected = false;
                UWVConnected = false;
            }
            bInitial = true;
        }
        public string ShipIpAddr
        {
            get { return GetPropertyValue(() => ShipIpAddr); }
            set { SetPropertyValue(() => ShipIpAddr, value); }
        }
        public string UWVIpAddr
        {
            get { return GetPropertyValue(() => UWVIpAddr); }
            set { SetPropertyValue(() => UWVIpAddr, value); }
        }
        public int SelectMode
        {
            get { return GetPropertyValue(() => SelectMode); }
            set
            {
                if (SelectMode != value && bInitial==true)
                {
                    EventAggregator.PublishMessage(new LogEvent("新的模式需在保存设置后生效！", LogType.OnlyInfo));
                }
                SetPropertyValue(() => SelectMode, value);
            }
        }
        public string Version
        {
            get { return GetPropertyValue(() => Version); }
            set { SetPropertyValue(() => Version, value); }
        }
        public bool IsFetching
        {
            get { return GetPropertyValue(() => IsFetching); }
            set { SetPropertyValue(() => IsFetching, value); }
        }
        public Visibility RefreshVisble
        {
            get { return GetPropertyValue(() => RefreshVisble); }
            set { SetPropertyValue(() => RefreshVisble, value); }
        }
        public bool ShipConnected
        {
            get { return GetPropertyValue(() => ShipConnected); }
            set { SetPropertyValue(() => ShipConnected, value); }
        }
        public bool UWVConnected
        {
            get { return GetPropertyValue(() => UWVConnected); }
            set { SetPropertyValue(() => UWVConnected, value); }
        }
        public bool IsUpdating
        {
            get { return GetPropertyValue(() => IsUpdating); }
            set { SetPropertyValue(() => IsUpdating, value); }
        }
        public int UpdatePercentange
        {
            get { return GetPropertyValue(() => UpdatePercentange); }
            set { SetPropertyValue(() => UpdatePercentange, value); }
        }
        public int XmtIndex
        {
            get { return GetPropertyValue(() => XmtIndex); }
            set { SetPropertyValue(() => XmtIndex, value); }
        }
        public float XMTValue
        {
            get { return GetPropertyValue(() => XMTValue); }
            set { SetPropertyValue(() => XMTValue, value); }
        }

        public int Gain
        {
            get { return GetPropertyValue(() => Gain); }
            set { SetPropertyValue(() => Gain, value); }
        }
        //// command
        public ICommand LinkCheckCommand
        {
            get { return GetPropertyValue(() => LinkCheckCommand); }
            set { SetPropertyValue(() => LinkCheckCommand, value); }
        }


        public void CanExecuteLinkCheckCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteLinkCheckCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if (UnitCore.Instance.NetCore.IsTCPWorking)
                return;
            else
            {
                try
                {
                    Save();
                    UnitCore.Instance.NetCore.StartTCPService();
                }
                catch (Exception e)
                {
                    EventAggregator.PublishMessage(new LogEvent(e.Message, LogType.OnlyInfo));
                }

                if (UnitCore.Instance.NetCore.IsTCPWorking)
                {
                    if (SelectMode == 0)
                    {
                        ShipConnected = true;
                        UWVConnected = false;
                    }
                    else
                    {
                        UWVConnected = true;
                        ShipConnected = false;
                    }
                }
                else
                {
                    UWVConnected = false;
                    ShipConnected = false;
                }

            }
        }
        public ICommand LinkUnCheckCommand
        {
            get { return GetPropertyValue(() => LinkUnCheckCommand); }
            set { SetPropertyValue(() => LinkUnCheckCommand, value); }
        }


        public void CanExecuteLinkUnCheckCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteLinkUnCheckCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if(UnitCore.Instance.NetCore.IsTCPWorking)
                UnitCore.Instance.NetCore.StopTCpService();
            UWVConnected = false;
            ShipConnected = false;
        }
        public ICommand FetchingVersion
        {
            get { return GetPropertyValue(() => FetchingVersion); }
            set { SetPropertyValue(() => FetchingVersion, value); }
        }


        public void CanExecuteFetchingVersion(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public async void ExecuteFetchingVersion(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            IsFetching = true;
            RefreshVisble = Visibility.Collapsed;
            //fetch data...
            if (UnitCore.Instance.NetCore.IsTCPWorking)
            {
                await UnitCore.Instance.NetCore.SendConsoleCMD("ver");
                await TaskEx.Delay(500);
                Version = UnitCore.Instance.Version;

            }
            else
            {
                IsFetching = false;
                RefreshVisble = Visibility.Visible;
            }
            IsFetching = false;
            RefreshVisble = Visibility.Visible;
        }

        public ICommand SaveConfig
        {
            get { return GetPropertyValue(() => SaveConfig); }
            set { SetPropertyValue(() => SaveConfig, value); }
        }


        public void CanExecuteSaveConfig(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public async void ExecuteSaveConfig(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            Save();
            UnitCore.Instance.WorkMode = (MonitorMode) Enum.Parse(typeof (MonitorMode), SelectMode.ToString());
            var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
            dialog.Title = "设置";
            await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                dialog);
            var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
            textBlock.Text = "修改成功！";
            await TaskEx.Delay(1000);
            await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, dialog);
        }

        private void Save()
        {
            if (UnitCore.Instance.NetCore.IsTCPWorking)
            {
                if (UnitCore.Instance.LoadConfiguration() == false)
                {
                    EventAggregator.PublishMessage(new LogEvent("读取配置出错！", LogType.Both));
                    return;
                }
                if (SelectMode != (int)UnitCore.Instance.MovConfigueService.GetMode())
                {
                    EventAggregator.PublishMessage(new LogEvent("修改运行模式前请先停止当前连接！", LogType.Both));
                    return;
                }
                if (ShipConnected && ShipIpAddr != UnitCore.Instance.MovConfigueService.GetShipIP())
                {
                    EventAggregator.PublishMessage(new LogEvent("修改母船网络地址前请先停止当前连接！", LogType.Both));
                    return;
                }
                if (UWVConnected && UWVIpAddr != UnitCore.Instance.MovConfigueService.GetUWVIP())
                {
                    EventAggregator.PublishMessage(new LogEvent("修改潜器网络地址前请先停止当前连接！", LogType.Both));
                    return;
                }
            }
            if(SelectMode != (int)UnitCore.Instance.MovConfigueService.GetMode())//不同的模式才要重新保存
            {
                var ret =
                    UnitCore.Instance.MovConfigueService.SetMode(
                        (MonitorMode) Enum.Parse(typeof (MonitorMode), SelectMode.ToString()));
                if (ret == false)
                {
                    EventAggregator.PublishMessage(new LogEvent("保存运行模式出错！", LogType.Both));
                    return;
                }
                ret =
                    UnitCore.Instance.MovTraceService.SetMode(
                        (MonitorMode) Enum.Parse(typeof (MonitorMode), SelectMode.ToString()));
                if (ret == false)
                {
                    EventAggregator.PublishMessage(new LogEvent("数据保存服务设置出错！", LogType.Both));
                    return;
                }
                ACM4500Protocol.Init((MonitorMode) Enum.Parse(typeof (MonitorMode), SelectMode.ToString()));
            }
            IPAddress ip;
            if (IPAddress.TryParse(ShipIpAddr, out ip) == false)
            {
                EventAggregator.PublishMessage(new LogEvent("母船通信机网络地址非法！", LogType.OnlyInfo));
                return;
            }
            if (IPAddress.TryParse(UWVIpAddr, out ip) == false)
            {
                EventAggregator.PublishMessage(new LogEvent("潜器通信机网络地址非法！", LogType.OnlyInfo));
                return;
            }
            bool result = (UnitCore.Instance.MovConfigueService.SetShipIP(ShipIpAddr) &&
                   UnitCore.Instance.MovConfigueService.SetUWVIP(UWVIpAddr));
            if (result == false)
            {
                EventAggregator.PublishMessage(new LogEvent("保存网络地址出错", LogType.Both));
                return;
            }
            var ans = UnitCore.Instance.MovConfigueService.SetXmtChannel(XmtIndex + 1) &&
                  UnitCore.Instance.MovConfigueService.SetXmtAmp(XMTValue) &&
                  UnitCore.Instance.MovConfigueService.SetGain(Gain);
            if (ans == false)
            {
                EventAggregator.PublishMessage(new LogEvent("保存参数出错", LogType.Both));
                return;
            }
        }

        private void ReConnectToDSP()
        {
            if (UnitCore.Instance.NetCore.IsTCPWorking)
            {
                if(UnitCore.Instance.NetCore.IsInitialize)
                    UnitCore.Instance.NetCore.StopTCpService();
            }
            try
            {
                if (!UnitCore.Instance.LoadConfiguration()) throw new Exception("无法读取网络配置");
                UnitCore.Instance.NetCore.StartTCPService();
            }
            catch (Exception e)
            {
                EventAggregator.PublishMessage(new LogEvent(UnitCore.Instance.NetCore.Error, LogType.OnlyInfo));
                return;
            }
        }
    }
}
