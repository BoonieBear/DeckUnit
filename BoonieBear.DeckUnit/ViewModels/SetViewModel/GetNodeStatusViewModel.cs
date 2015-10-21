using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using BoonieBear.DeckUnit.ACNP;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.Events;
using BoonieBear.DeckUnit.Helps;
using MahApps.Metro.Controls.Dialogs;
using TinyMetroWpfLibrary.EventAggregation;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;

namespace BoonieBear.DeckUnit.ViewModels.SetViewModel
{
    public class GetNodeStatusViewModel : ViewModelBase, IHandleMessage<NodeStatusInfo>
    {
        public override void Initialize()
        {
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            SendCMD = RegisterCommand(ExecuteSendCMD, CanExecuteSendCMD, true);
            IsProcessing = false;
            Info = string.Empty;
        }

        public override void InitializePage(object extraData)
        {
            
        }

        public string Info
        {
            get { return GetPropertyValue(() => Info); }
            set { SetPropertyValue(() => Info, value); }
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
            IsProcessing = true;
            var cmd = MSPHexBuilder.Pack247();
            Task<bool> result;
            result = UnitCore.Instance.CommEngine.SendCMD(cmd);
            await result;
            var ret = result.Result;
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "确定";
            if (ret == false)
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "发送失败",
                UnitCore.Instance.CommEngine.Error,MessageDialogStyle.Affirmative,md);
            else
            {
                //await TaskEx.Delay(1000);
                //等待数据更新
            }
            IsProcessing = false;
        }
        #endregion

        public void Handle(NodeStatusInfo message)
        {
            Info = message.info;
        }
    }
}
