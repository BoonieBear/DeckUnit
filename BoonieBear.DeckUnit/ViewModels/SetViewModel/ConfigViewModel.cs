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
    public class ConfigViewModel : ViewModelBase
    {
        public override void Initialize()
        {
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            GoSetDateView = RegisterCommand(ExecuteGoSetDateView, CanExecuteGoSetDateView, true);
            GoSetSleepView = RegisterCommand(ExecuteGoSetSleepView, CanExecuteGoSetSleepView, true);
            ShowSetADView = RegisterCommand(ExecuteShowSetADView, CanExecuteShowSetADView, true);
            ShowWakeUpView = RegisterCommand(ExecuteShowWakeUpView, CanExecuteShowWakeUpView, true);
            ShowSupplyView = RegisterCommand(ExecuteShowSupplyView, CanExecuteShowSupplyView, true);
            IsProcessing = false;
        }

        public override void InitializePage(object extraData)
        {
           
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

        public ICommand GoSetDateView
        {
            get { return GetPropertyValue(() => GoSetDateView); }
            set { SetPropertyValue(() => GoSetDateView, value); }
        }


        private void CanExecuteGoSetDateView(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private void ExecuteGoSetDateView(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoSetDateTimePage("设置系统时间"));
        }
        public ICommand GoSetSleepView
        {
            get { return GetPropertyValue(() => GoSetSleepView); }
            set { SetPropertyValue(() => GoSetSleepView, value); }
        }


        private void CanExecuteGoSetSleepView(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private void ExecuteGoSetSleepView(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoSetDateTimePage("设置休眠时间"));
        }

        public ICommand ShowSetADView
        {
            get { return GetPropertyValue(() => ShowSetADView); }
            set { SetPropertyValue(() => ShowSetADView, value); }
        }


        private void CanExecuteShowSetADView(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private async void ExecuteShowSetADView(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["SetADDialog"];
            await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                dialog);
        }
        public ICommand ShowWakeUpView
        {
            get { return GetPropertyValue(() => ShowWakeUpView); }
            set { SetPropertyValue(() => ShowWakeUpView, value); }
        }


        private void CanExecuteShowWakeUpView(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private async void ExecuteShowWakeUpView(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["SetWakeUpDialog"];
            await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                dialog);
        }
        public ICommand ShowSupplyView
        {
            get { return GetPropertyValue(() => ShowSupplyView); }
            set { SetPropertyValue(() => ShowSupplyView, value); }
        }


        private void CanExecuteShowSupplyView(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private async void ExecuteShowSupplyView(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["SupplySetDialog"];
            await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                dialog);
        }
        #endregion
    }
}
