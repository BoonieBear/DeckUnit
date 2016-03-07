using System;
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
using System.Globalization;

namespace BoonieBear.DeckUnit.ViewModels.SetViewModel
{
    public class SetDateTimeViewModel : ViewModelBase
    {

        public override void Initialize()
        {
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            SendCMD = RegisterCommand(ExecuteSendCMD, CanExecuteSendCMD, true);
            SelectedDate = DateTime.Today;
            SelectedTime = DateTime.Now;
        }

        public override void InitializePage(object extraData)
        {

 	        var title = extraData as string;
            if (title != null)
                Title = title;
            
        }
        public String Title
        {
            get { return GetPropertyValue(() => Title); }
            set { SetPropertyValue(() => Title, value); }
        }
        //命令处理中
        public bool IsProcessing
        {
            get { return GetPropertyValue(() => IsProcessing); }
            set { SetPropertyValue(() => IsProcessing, value); }
        }

        public DateTime SelectedDate
        {
            get { return GetPropertyValue(() => SelectedDate); }
            set { SetPropertyValue(() => SelectedDate, value); }
        }
        public DateTime SelectedTime
        {
            get { return GetPropertyValue(() => SelectedTime); }
            set { SetPropertyValue(() => SelectedTime, value); }
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
            Task<bool> result = null;
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "确定";
            DateTime dt = new DateTime(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day, SelectedTime.Hour, SelectedTime.Minute, 0);
            if (dt<DateTime.Now)
            {
                
                await
                    MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame,
                        "发送失败",
                       "选择时间无效", MessageDialogStyle.Affirmative, md);
                return;
            }
            IsProcessing = true;
            switch (Title)
            {
                case "设置系统时间":
                    result = SetSystemTime(dt);
                    break;
                case "设置休眠时间":
                    result = SetSleepTime(dt);
                    break;
                default:
                    break;
            }
            await result;
            ret = result.Result;
            IsProcessing = false;

            if (ret == false)
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "发送失败",
                UnitCore.Instance.CommEngine.Error,MessageDialogStyle.Affirmative,md);
            else
            {
                var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                dialog.Title = Title;
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                    dialog);

                var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
                textBlock.Text = "发送成功！";

                await TaskEx.Delay(2000);

                await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, dialog);
            }
        }

        private Task<bool> SetSleepTime(DateTime dt)
        {
            
            var cmd = MSPHexBuilder.Pack251(dt);
            return UnitCore.Instance.CommEngine.SendCMD(cmd);
        }
        private Task<bool> SetSystemTime(DateTime dt)
        {
            
            var cmd = MSPHexBuilder.Pack252(dt);
            return UnitCore.Instance.CommEngine.SendCMD(cmd);

        }
    }
        #endregion
}
