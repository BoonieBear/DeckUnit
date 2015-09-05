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
using System.Windows;
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
            ID = 1;
        }

        public override void InitializePage(object extraData)
        {
            if (UnitCore.Instance.LoadConfiguration())
            {
                SelectComm = CommInfo.IndexOf(DeckUnitConf.GetInstance().GetCommConfInfo().SerialPort);
                var ip = DeckUnitConf.GetInstance().GetCommConfInfo().LinkIP;
                IPaddr = int.Parse(ip.Substring(ip.LastIndexOf(".") + 1));
                ID = DeckUnitConf.GetInstance().GetModemConfigure().ID;
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
        public int ID
        {
            get { return GetPropertyValue(() => ID); }
            set { SetPropertyValue(() => ID, value); }
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
            var moderm = DeckUnitConf.GetInstance().GetModemConfigure();
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "确定";
            md.NegativeButtonText = "取消";
            if (CommInfo.Count > 0&&cominfo!=null)
            {
                if ((cominfo.SerialPort != CommInfo[SelectComm]) || (cominfo.LinkIP != "192.168.2." + IPaddr.ToString()) ||
                    (moderm.ID != ID)) //有改变
                {
                    MessageDialogResult result = await  MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "改变设置",
                    "选择确定将重新配置系统，程序将自动重启", MessageDialogStyle.AffirmativeAndNegative, md);
                    if (result != MessageDialogResult.Affirmative)//取消
                    {
                        IsProcessing = false;
                        return;
                    }
                                     
                }
                else
                {
                    var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                    dialog.Title = "设置命令";
                    await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                        dialog);

                    var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
                    textBlock.Text = "没有任何改变项，当前连接不变！";

                    await TaskEx.Delay(1000);
                    IsProcessing = false;
                    await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, dialog);
                    return;
                }
                cominfo.SerialPort = CommInfo[SelectComm];
                cominfo.LinkIP = "192.168.2." + IPaddr.ToString();
                moderm.ID = ID;
                bool ret = false;
                string err = string.Empty;
                
                try
                {
                    ret = (DeckUnitConf.GetInstance().UpdateCommSet(cominfo) &&
                           DeckUnitConf.GetInstance().UpdateModemSet(moderm));
                    if (ret == false)
                        err = "写入数据库失败！";
                }
                catch (Exception ex)
                {
                    ret = false;
                    err = ex.Message;
                }
                
                if (ret == false)
                    await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "设置失败",
                    err,MessageDialogStyle.Affirmative,md);
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
                    System.Windows.Forms.Application.Restart();
                    Application.Current.Shutdown();
                }
            }
            else
            {
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "无法设置串口",
                "串口不存在或没有已保存的设置",MessageDialogStyle.Affirmative,md);
            }
            IsProcessing = false;
        }
        #endregion

    }
}
