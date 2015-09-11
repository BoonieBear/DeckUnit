using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using BoonieBear.DeckUnit.ACNP;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.Helps;
using MahApps.Metro.Controls.Dialogs;
using TinyMetroWpfLibrary.EventAggregation;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;
using BoonieBear.DeckUnit.Events;
using BoonieBear.DeckUnit.BaseType;
namespace BoonieBear.DeckUnit.ViewModels.SetViewModel
{
    public class PingTestViewModel : ViewModelBase, IHandleMessage<PingNotifyEvent>
    {
        public override void Initialize()
        {
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            SendCMD = RegisterCommand(ExecuteSendCMD, CanExecuteSendCMD, true);
            ID = new List<FilterItem>();
            for (int i = 1; i < 64; i++)
            {
                ID.Add(new FilterItem(i.ToString())); ;
            }
            NodeID = ID[4];
            IsReceived = false;
            index = -1;
        }

        public override void InitializePage(object extraData)
        {

        }
        public int index
        {
            get { return GetPropertyValue(() => index); }
            set { SetPropertyValue(() => index, value); }
        }
        public List<FilterItem> ID
        {
            get { return GetPropertyValue(() => ID); }
            set { SetPropertyValue(() => ID, value); }
        }
        //节点号
        public FilterItem NodeID
        {
            get { return GetPropertyValue(() => NodeID); }
            set { SetPropertyValue(() => NodeID, value); }
        }
        public string PingCMD
        {
            get { return GetPropertyValue(() => PingCMD); }
            set { SetPropertyValue(() => PingCMD, value); }
        }
        public string StartTime
        {
            get { return GetPropertyValue(() => StartTime); }
            set { SetPropertyValue(() => StartTime, value); }
        }
      
        public string PingDATA
        {
            get { return GetPropertyValue(() => PingDATA); }
            set { SetPropertyValue(() => PingDATA, value); }
        }
        public string RecvTime
        {
            get { return GetPropertyValue(() => RecvTime); }
            set { SetPropertyValue(() => RecvTime, value); }
        }
        
        public bool IsReceived
        {
            get { return GetPropertyValue(() => IsReceived); }
            set { SetPropertyValue(() => IsReceived, value); }

        }
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
            eventArgs.CanExecute = true;
        }


        private async void ExecuteSendCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            RecvTime = "";
            PingDATA = "";
            IsProcessing = true;
            ACNBuilder.Ping(NodeID.num, PingCMD);
            var cmd = ACNProtocol.Package(false);
            var cl = new CommandLog();
            cl.DestID = NodeID.num;
            cl.SourceID = (int)ACNProtocol.SourceID;
            cl.LogTime = DateTime.Now;
            cl.CommID = 101;
            cl.Type = false;
            UnitCore.Instance.UnitTraceService.Save(cl, cmd);
            var result = UnitCore.Instance.NetEngine.SendCMD(cmd);
            await result;
            bool ret = result.Result;
            IsProcessing = false;
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "好的";
            if (ret == false)
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "发送失败",
                UnitCore.Instance.NetEngine.Error, MessageDialogStyle.Affirmative, md);
            else
            {
                StartTime = "开始测试："+DateTime.Now.ToString();
                ProgressDialogController remote = null;
                md.AffirmativeButtonText = "取消";
                md.NegativeButtonText = "取消";
                var processcontroller =
                    await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowProgressAsync(MainFrameViewModel.pMainFrame,
                        "测试开始", "数据包已发送，等待节点回传……",true,md);
                await TaskEx.Delay(1000);
                int i = 1;
                while (IsReceived==false)
                {
                    processcontroller.SetMessage("已等待: " + i + "秒");

                    if (processcontroller.IsCanceled||IsReceived)
                        break; 
                    i += 1;
                    await TaskEx.Delay(1000);
                }
                await processcontroller.CloseAsync();
                md.AffirmativeButtonText = "好的";
                if (processcontroller.IsCanceled)
                {
                    await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "取消等待", "本次测试结果将保留",
                        MessageDialogStyle.Affirmative, md);
                }
                else//收到数据了
                {
                    RecvTime = "收到回环包："+DateTime.Now.ToString("s");
                    await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame,"收到数据!", "请查看测试结果",
                         MessageDialogStyle.Affirmative, md);
                }
            }
        }
        #endregion

        public void Handle(PingNotifyEvent message)
        {
            IsReceived = true;
            PingDATA = message.info;
        }
    }
}
