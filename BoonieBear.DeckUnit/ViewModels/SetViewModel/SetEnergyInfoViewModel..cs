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
using BoonieBear.DeckUnit.Events;
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
            
            var pw = new PWState();
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
            p48state.LowCurrentTime = (uint)LowTime;
            p48state.MediumCurrentTime = (uint)MiddleTime;
            p48state.HighCurrentTime = (uint)HighTime;
            if (LowTime < 0 || LowTime > uint.MaxValue)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("低电流时间无效（0-4294967295）", LogType.OnlyInfo));
                return;
            }
            if (MiddleTime < 0 || MiddleTime > uint.MaxValue)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("中电流时间无效（0-4294967295）", LogType.OnlyInfo));
                return;
            }
            if (HighTime < 0 || HighTime > uint.MaxValue)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("高电流时间无效（0-4294967295）", LogType.OnlyInfo));
                return;
            }
            if (SleepTime < 0 || SleepTime > ushort.MaxValue)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("休眠时间无效（0-65535）", LogType.OnlyInfo));
                return;
            }
            if (WorkTime < 0 || WorkTime > ushort.MaxValue)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("工作时间无效（0-65535）", LogType.OnlyInfo));
                return;
            }
            if (v33Votage < 0 || v33Votage > 9.999)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("3.3V无效（0-9.999）", LogType.OnlyInfo));
                return;
            }
            if (v48Votage < 0 || v48Votage > 999.999)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("48V无效（0-999.999）", LogType.OnlyInfo));
                return;
            }
            if (v48Left < 0 || v48Left > 50000)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("48V剩余电量无效（0-50000）", LogType.OnlyInfo));
                return;
            }
            if (v48Used < 0 || v48Used > 50000)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("48V已用电量无效（0-50000）", LogType.OnlyInfo));
                return;
            }
            if (v3Left < 0 || v3Left > 68000)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("3.3V剩余电量无效（0-68000）", LogType.OnlyInfo));
                return;
            }
            if (v3Used < 0 || v3Used > 68000)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("3.3V已用电量无效（0-68000）", LogType.OnlyInfo));
                return;
            }
            Task<bool> result;
            IsProcessing = true;
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
