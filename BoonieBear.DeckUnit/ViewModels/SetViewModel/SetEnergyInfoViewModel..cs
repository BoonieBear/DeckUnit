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
using BoonieBear.DeckUnit.Helps;
using MahApps.Metro.Controls.Dialogs;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;

namespace BoonieBear.DeckUnit.ViewModels.SetViewModel
{
    public class SetEnergyInfoViewModel : ViewModelBase
    {
        public override void Initialize()
        {
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            SendCMD = RegisterCommand(ExecuteSendCMD, CanExecuteSendCMD, true);
        }

        public override void InitializePage(object extraData)
        {
            
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

        public double LowTime
        {
            get { return GetPropertyValue(() => LowTime); }
            set { SetPropertyValue(() => LowTime, value); }
        }
        public double MiddleTime
        {
            get { return GetPropertyValue(() => MiddleTime); }
            set { SetPropertyValue(() => MiddleTime, value); }
        }
        public double HighTime
        {
            get { return GetPropertyValue(() => HighTime); }
            set { SetPropertyValue(() => HighTime, value); }
        }
        public double SleepTime
        {
            get { return GetPropertyValue(() => SleepTime); }
            set { SetPropertyValue(() => SleepTime, value); }
        }
        public double WorkTime
        {
            get { return GetPropertyValue(() => WorkTime); }
            set { SetPropertyValue(() => WorkTime, value); }
        }
        public double v33Votage
        {
            get { return GetPropertyValue(() => v33Votage); }
            set { SetPropertyValue(() => v33Votage, value); }
        }
        public double v48Votage
        {
            get { return GetPropertyValue(() => v48Votage); }
            set { SetPropertyValue(() => v48Votage, value); }
        }
        public double v48Left
        {
            get { return GetPropertyValue(() => v48Left); }
            set { SetPropertyValue(() => v48Left, value); }
        }
        public double v48Used
        {
            get { return GetPropertyValue(() => v48Used); }
            set { SetPropertyValue(() => v48Used, value); }
        }
        public double v3Left
        {
            get { return GetPropertyValue(() => v3Left); }
            set { SetPropertyValue(() => v3Left, value); }
        }
        public double v3Used
        {
            get { return GetPropertyValue(() => v3Used); }
            set { SetPropertyValue(() => v3Used, value); }
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
            var pw = new PWState();
            Task<bool> result;
            pw.Voltage33 = v33Votage;
            pw.Voltage48 = v48Votage;
            pw.Pw48Left = v48Left;
            pw.Pw48Consume = v48Used;
            pw.Pw33Left = v3Left;
            pw.Pw33Consume = v3Used;
            var mspstate = new MSPWorkState();
            mspstate.Sleeptime = (int)SleepTime;
            mspstate.Worktime = (int)WorkTime;
            var p48state = new Power48VState();
            p48state.LowCurrentTime = (int)LowTime;
            p48state.MediumCurrentTime = (int)MiddleTime;
            p48state.HighCurrentTime = (int)HighTime;
            var cmd = MSPHexBuilder.Pack254(p48state, mspstate, pw);
            result = UnitCore.Instance.CommEngine.SendCMD(cmd);
            await result;
            bool ret = result.Result;
            IsProcessing = false;
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "确定";
            if (ret == false)
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "发送失败",
                UnitCore.Instance.NetEngine.Error,MessageDialogStyle.Affirmative,md);
            else
            {
                var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                dialog.Title = "设置能量";
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                    dialog);

                var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
                textBlock.Text = "发送成功！";

                await TaskEx.Delay(2000);

                await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, dialog);
            }
        }
        #endregion
    }
}
