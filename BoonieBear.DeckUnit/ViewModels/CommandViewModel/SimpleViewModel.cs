using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using System.Windows.Navigation;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.Events;

using MahApps.Metro.Controls.Dialogs;
using TinyMetroWpfLibrary.Events;
using System.Text;
using TinyMetroWpfLibrary;
using TinyMetroWpfLibrary.ViewModel;
using BoonieBear.DeckUnit.ACNP;
using System.Threading.Tasks;
using BoonieBear.DeckUnit.Helps;
using BoonieBear.DeckUnit.Views;
using System.Windows.Controls;
using BoonieBear.DeckUnit.BaseType;
namespace BoonieBear.DeckUnit.ViewModels.CommandViewModel
{
    public class SimpleViewModel : ViewModelBase
    {
       
        public override void Initialize()
        {

            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            SendCMD = RegisterCommand(ExecuteSendCMD, CanExecuteSendCMD, true);
            ID=new List<FilterItem>();
            for (int i = 1; i < 64; i++)
            {
                ID.Add(new FilterItem(i.ToString()));;
            }
            NodeID = ID[0];
            ShowComm = false;
            ShowRebuild = false;
            CommSelect = 2;
            RebuildRequired = false;
            IsProcessing = false;
        }

        public override void InitializePage(object extraData)
        {
            
            var title = extraData as string;
            if (title != null)
                Title = title;
            switch (Title)
            {
                case "获取设备状态":
                    ShowComm = true;
                    break;
                case "获取设备数据":
                    ShowComm = true;
                    break;
                case "获取邻接点表":
                    ShowRebuild = true;
                    break;
                case "获取网络表":
                    ShowRebuild = true;
                    break;
                default:
                    break;
            }
        }
        #region 绑定属性

        public List<FilterItem> ID
        {
            get { return GetPropertyValue(() => ID); }
            set { SetPropertyValue(() => ID, value); }
        }
                
        public String Title
        {
            get { return GetPropertyValue(() => Title); }
            set { SetPropertyValue(() => Title, value); }
        }
        #endregion
        //是否显示串口选择
        public bool ShowComm
        {
            get { return GetPropertyValue(() => ShowComm); }
            set { SetPropertyValue(() => ShowComm, value); }
        }
        //节点号
        public FilterItem NodeID
        {
            get { return GetPropertyValue(() => NodeID); }
            set { SetPropertyValue(() => NodeID, value); }
        }
        //是否重建
        public bool ShowRebuild
        {
            get { return GetPropertyValue(() => ShowRebuild); }
            set { SetPropertyValue(() => ShowRebuild, value); }
        }
        //需要重建
        public bool RebuildRequired
        {
            get { return GetPropertyValue(() => RebuildRequired); }
            set { SetPropertyValue(() => RebuildRequired, value); }
        }
        //选择的串口号
        public int CommSelect
        {
            get { return GetPropertyValue(() => CommSelect); }
            set { SetPropertyValue(() => CommSelect, value); }
        }
        //命令处理中
        public bool IsProcessing
        {
            get { return GetPropertyValue(() => IsProcessing); }
            set { SetPropertyValue(() => IsProcessing, value); }
        }

        #region cmd
        public ICommand GoBackCommand
        {
            get { return GetPropertyValue(() => GoBackCommand); }
            set { SetPropertyValue(() => GoBackCommand, value); }
        }


        private void CanExecuteGoBackCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private void ExecuteGoBackCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }
        public ICommand SendCMD
        {
            get { return GetPropertyValue(() => SendCMD); }
            set { SetPropertyValue(() => SendCMD, value); }
        }


        private void CanExecuteSendCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = !IsProcessing;
        }


        private async void ExecuteSendCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            bool ret = false;
            IsProcessing = true;
            Task<bool> result = null;
            switch (Title)
            {
                 case "获取设备状态":
                    result = GetDeviceStatus();
                    break;
                case "获取设备数据":
                    result = GetDeviceData();
                    break;
                case "获取邻接点表":
                    result = GetNeiborList();
                    break;
                case "获取网络表":
                    result = GetNetList();
                    break;
                case "获取网络简表":
                    result = GetSimpleNetList();
                    break;
                case "获取节点状态":
                    result = GetNodeStatus();
                    break;
                case "获取节点信息":
                    result = GetNodeInfo();
                    break;
                case "获取节点信息表":
                    result = GetNodeInfoList();
                    break;
                case "获取节点路由":
                    result = GetNodeRoute();
                    break;
                default:
                    break;
            }
            await result;
            ret = result.Result;
            IsProcessing = false;
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "确定";
            if(ret==false)
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "发送失败",
                UnitCore.Instance.NetEngine.Error,MessageDialogStyle.Affirmative,md);
            else
            {
                var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                dialog.Title = "设备命令";
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                    dialog);

                var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
                textBlock.Text = "发送成功！";

                await TaskEx.Delay(2000);

                await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame,dialog);
            }
        }

        private Task<bool> GetDeviceData()
        {
            ACNBuilder.Pack115(NodeID.num,CommSelect);
            var cmd = ACNProtocol.Package(false);
            var cl = new CommandLog();
            cl.DestID = NodeID.num;
            cl.SourceID = (int)ACNProtocol.SourceID;
            cl.LogTime = DateTime.Now;
            cl.CommID = 115;
            cl.Type = false;
            UnitCore.Instance.UnitTraceService.Save(cl, cmd);
            return  UnitCore.Instance.NetEngine.SendCMD(cmd);

        }

        private Task<bool> GetDeviceStatus()
        {
            ACNBuilder.Pack117(NodeID.num, CommSelect);
            var cmd = ACNProtocol.Package(false);
            var cl = new CommandLog();
            cl.DestID = NodeID.num;
            cl.SourceID = (int)ACNProtocol.SourceID;
            cl.LogTime = DateTime.Now;
            cl.CommID = 117;
            cl.Type = false;
            UnitCore.Instance.UnitTraceService.Save(cl, cmd);
            return UnitCore.Instance.NetEngine.SendCMD(cmd);
        }

        private Task<bool> GetNeiborList()
        {
            ACNBuilder.Pack111(NodeID.num, RebuildRequired);
            var cmd = ACNProtocol.Package(false);
            var cl = new CommandLog();
            cl.DestID = NodeID.num;
            cl.SourceID = (int)ACNProtocol.SourceID;
            cl.LogTime = DateTime.Now;
            cl.CommID = 111;
            cl.Type = false;
            UnitCore.Instance.UnitTraceService.Save(cl, cmd);
            return UnitCore.Instance.NetEngine.SendCMD(cmd);
        }
        private Task<bool> GetNetList()
        {
            ACNBuilder.Pack107(NodeID.num, RebuildRequired);
            var cmd = ACNProtocol.Package(false);
            var cl = new CommandLog();
            cl.DestID = NodeID.num;
            cl.SourceID = (int)ACNProtocol.SourceID;
            cl.LogTime = DateTime.Now;
            cl.CommID = 107;
            cl.Type = false;
            UnitCore.Instance.UnitTraceService.Save(cl, cmd);
            return UnitCore.Instance.NetEngine.SendCMD(cmd);
        }
        private Task<bool> GetSimpleNetList()
        {
            ACNBuilder.Pack109(NodeID.num);
            var cmd = ACNProtocol.Package(false);
            var cl = new CommandLog();
            cl.DestID = NodeID.num;
            cl.SourceID = (int)ACNProtocol.SourceID;
            cl.LogTime = DateTime.Now;
            cl.CommID = 109;
            cl.Type = false;
            UnitCore.Instance.UnitTraceService.Save(cl, cmd);
            return UnitCore.Instance.NetEngine.SendCMD(cmd);
        }
        private Task<bool> GetNodeStatus()
        {
            ACNBuilder.Pack121(NodeID.num);
            var cmd = ACNProtocol.Package(false);
            var cl = new CommandLog();
            cl.DestID = NodeID.num;
            cl.SourceID = (int)ACNProtocol.SourceID;
            cl.LogTime = DateTime.Now;
            cl.CommID = 121;
            cl.Type = false;
            UnitCore.Instance.UnitTraceService.Save(cl, cmd);
            return UnitCore.Instance.NetEngine.SendCMD(cmd);
        }
        private Task<bool> GetNodeInfo()
        {
            ACNBuilder.Pack103(NodeID.num);
            var cmd = ACNProtocol.Package(false);
            var cl = new CommandLog();
            cl.DestID = NodeID.num;
            cl.SourceID = (int)ACNProtocol.SourceID;
            cl.LogTime = DateTime.Now;
            cl.CommID = 103;
            cl.Type = false;
            UnitCore.Instance.UnitTraceService.Save(cl, cmd);
            return UnitCore.Instance.NetEngine.SendCMD(cmd);
        }
        private Task<bool> GetNodeInfoList()
        {
            ACNBuilder.Pack105(NodeID.num);
            var cmd = ACNProtocol.Package(false);
            var cl = new CommandLog();
            cl.DestID = NodeID.num;
            cl.SourceID = (int)ACNProtocol.SourceID;
            cl.LogTime = DateTime.Now;
            cl.CommID = 105;
            cl.Type = false;
            UnitCore.Instance.UnitTraceService.Save(cl, cmd);
            return UnitCore.Instance.NetEngine.SendCMD(cmd);
        }
        private Task<bool> GetNodeRoute()
        {
            ACNBuilder.Pack113(NodeID.num);
            var cmd = ACNProtocol.Package(false);
            var cl = new CommandLog();
            cl.DestID = NodeID.num;
            cl.SourceID = (int)ACNProtocol.SourceID;
            cl.LogTime = DateTime.Now;
            cl.CommID = 113;
            cl.Type = false;
            UnitCore.Instance.UnitTraceService.Save(cl, cmd);
            return UnitCore.Instance.NetEngine.SendCMD(cmd);
        }
        #endregion
    }
}
