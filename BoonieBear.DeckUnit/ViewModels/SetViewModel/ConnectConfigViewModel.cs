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
using BoonieBear.DeckUnit.DUConf;
using BoonieBear.DeckUnit.Helps;
using MahApps.Metro.Controls.Dialogs;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;
using System.IO.Ports;
namespace BoonieBear.DeckUnit.ViewModels.SetViewModel
{
    public class ConnectConfigViewModel : ViewModelBase
    {
        public override void Initialize()
        {
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            SendCMD = RegisterCommand(ExecuteSendCMD, CanExecuteSendCMD, true);
            IsProcessing = false;
            CommInfo = SerialPort.GetPortNames().ToList();
            SelectComm = 0;
            IPaddr = 100;
            
        }

        public override void InitializePage(object extraData)
        {
            if (UnitCore.Instance.LoadConfiguration())
            {
                SelectComm = CommInfo.IndexOf(DeckUnitConf.GetInstance().GetCommConfInfo().SerialPort);
                var ip = DeckUnitConf.GetInstance().GetCommConfInfo().LinkIP;
                IPaddr = int.Parse(ip.Substring(ip.LastIndexOf(".") + 1));
            }
        }

        public bool IsProcessing
        {
            get { return GetPropertyValue(() => IsProcessing); }
            set { SetPropertyValue(() => IsProcessing, value); }

        }

        public int SelectComm
        {
            get { return GetPropertyValue(() => SelectComm); }
            set { SetPropertyValue(() => SelectComm, value); }
        }

        public int IPaddr
        {
            get { return GetPropertyValue(() => IPaddr); }
            set { SetPropertyValue(() => IPaddr, value); }
        }
        public List<string> CommInfo
        {
            get { return GetPropertyValue(() => CommInfo); }
            set { SetPropertyValue(() => CommInfo, value); }
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
            var cominfo = DeckUnitConf.GetInstance().GetCommConfInfo();
            if (CommInfo.Count > 0&&cominfo!=null)
            {
                cominfo.SerialPort = CommInfo[SelectComm];
                cominfo.LinkIP = "192.169.2." + IPaddr.ToString();
                bool ret = false;
                string err = string.Empty;
                try
                {
                    ret = DeckUnitConf.GetInstance().UpdateCommSet(cominfo);
                }
                catch (Exception ex)
                {
                    err = ex.Message;
                }
                if (ret == false)
                    await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "设置失败",
                    err);
                else
                {
                    var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                    dialog.Title = "设置命令";
                    await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                        dialog);

                    var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
                    textBlock.Text = "设置成功！";

                    await TaskEx.Delay(2000);

                    await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, dialog);
                }
            }
            else
            {
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "无法设置串口",
                "当前系统不存在串口或无法存储串口");
            }
            IsProcessing = false;
        }
        #endregion

    }
}
