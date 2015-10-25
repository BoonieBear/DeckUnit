using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using BoonieBear.DeckUnit.Events;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;

namespace BoonieBear.DeckUnit.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Overrides of ViewModelBase


        public override void Initialize()
        {
            GoAcousticViewCMD = RegisterCommand(ExecuteGoAcousticViewCMD, CanExecuteGoAcousticViewCMD, true);
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            GoNewTaskViewCMD = RegisterCommand(ExecuteGoNewTaskViewCMD, CanExecuteGoNewTaskViewCMD, true);
            GoTaskListViewCMD = RegisterCommand(ExecuteGoTaskListViewCMD, CanExecuteGoTaskListViewCMD, true);
            //GoADCPDataViewCMD = RegisterCommand(ExecuteGoADCPDataViewCMD, CanExecuteGoADCPDataViewCMD, true);
            GoBasicConfigViewCMD = RegisterCommand(ExecuteGoBasicConfigViewCMD, CanExecuteGoBasicConfigViewCMD, true);
            GoConnectConfigViewCMD = RegisterCommand(ExecuteGoConnectConfigViewCMD, CanExecuteGoConnectConfigViewCMD,
                true);
            GoSetEnergyViewCMD = RegisterCommand(ExecuteGoSetEnergyViewCMD, CanExecuteGoSetEnergyViewCMD, true);
            GoRefreshConfigViewCMD = RegisterCommand(ExecuteGoRefreshConfigViewCMD, CanExecuteGoRefreshConfigViewCMD,
                true);
            GoHistoryCMDViewCMD = RegisterCommand(ExecuteGoHistoryCMDViewCMD, CanExecuteGoHistoryCMDViewCMD, true);
            GoPingViewCMD = RegisterCommand(ExecuteGoPingViewCMD, CanExecuteGoPingViewCMD, true);
            GoDebugViewCMD = RegisterCommand(ExecuteGoDebugViewCMD, CanExecuteGoDebugViewCMD, true);
            GoGetInfoViewCMD = RegisterCommand(ExecuteGoGetInfoViewCMD, CanExecuteGoGetInfoViewCMD, true);
            GoADViewCMD = RegisterCommand(ExecuteGoADViewCMD, CanExecuteGoADViewCMD, true);
            AddPropertyChangedNotification(() => NowTime);
            var t = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, Tick, Dispatcher.CurrentDispatcher);
        }

        void Tick(object sender, EventArgs e)
        {
            NowTime = DateTime.Now.ToString();
        }


        public override void InitializePage(object extraData)
        {
            
        }

        #endregion

		#region 绑定属性
		public string NowTime
		{
            get { return GetPropertyValue(() => NowTime); }
            set { SetPropertyValue(() => NowTime, value); }
		}
		

		#endregion

        #region GoBack Command


        public ICommand GoBackCommand
        {
            get { return GetPropertyValue(() => GoBackCommand); }
            set { SetPropertyValue(() => GoBackCommand, value); }
        }


        public void CanExecuteGoBackCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoBackCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }

        #endregion

        #region GoAcousticViewCMD


        public ICommand GoAcousticViewCMD
        {
            get { return GetPropertyValue(() => GoAcousticViewCMD); }
            set { SetPropertyValue(() => GoAcousticViewCMD, value); }
        }


        public void CanExecuteGoAcousticViewCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoAcousticViewCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            
            EventAggregator.PublishMessage(new GoAcousticViewNavigationEvent());
        }



        #endregion

        public ICommand GoNewTaskViewCMD
        {
            get { return GetPropertyValue(() => GoNewTaskViewCMD); }
            set { SetPropertyValue(() => GoNewTaskViewCMD, value); }
        }


        public void CanExecuteGoNewTaskViewCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoNewTaskViewCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            
            EventAggregator.PublishMessage(new GoNewTaskViewEvent());
        }
        public ICommand GoTaskListViewCMD
        {
            get { return GetPropertyValue(() => GoTaskListViewCMD); }
            set { SetPropertyValue(() => GoTaskListViewCMD, value); }
        }


        public void CanExecuteGoTaskListViewCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoTaskListViewCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            
            EventAggregator.PublishMessage(new GoTaskListViewEvent());
        }
        /*
        public ICommand GoADCPDataViewCMD
        {
            get { return GetPropertyValue(() => GoADCPDataViewCMD); }
            set { SetPropertyValue(() => GoADCPDataViewCMD, value); }
        }


        public void CanExecuteGoADCPDataViewCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoADCPDataViewCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {

            EventAggregator.PublishMessage(new GoADCPDataViewEvent());
        }*/
        public ICommand GoBasicConfigViewCMD
        {
            get { return GetPropertyValue(() => GoBasicConfigViewCMD); }
            set { SetPropertyValue(() => GoBasicConfigViewCMD, value); }
        }


        public void CanExecuteGoBasicConfigViewCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoBasicConfigViewCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            
            EventAggregator.PublishMessage(new GoCofigViewPage());
        }
        
        public ICommand GoConnectConfigViewCMD
        {
            get { return GetPropertyValue(() => GoConnectConfigViewCMD); }
            set { SetPropertyValue(() => GoConnectConfigViewCMD, value); }
        }


        public void CanExecuteGoConnectConfigViewCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoConnectConfigViewCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            
            EventAggregator.PublishMessage(new GoConnectConfigViewEvent());
        }
        public ICommand GoSetEnergyViewCMD
        {
            get { return GetPropertyValue(() => GoSetEnergyViewCMD); }
            set { SetPropertyValue(() => GoSetEnergyViewCMD, value); }
        }


        public void CanExecuteGoSetEnergyViewCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoSetEnergyViewCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            
            EventAggregator.PublishMessage(new GoSetEnergyViewEvent());
        }
        public ICommand GoRefreshConfigViewCMD
        {
            get { return GetPropertyValue(() => GoRefreshConfigViewCMD); }
            set { SetPropertyValue(() => GoRefreshConfigViewCMD, value); }
        }


        public void CanExecuteGoRefreshConfigViewCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoRefreshConfigViewCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            
            EventAggregator.PublishMessage(new GoRefreshNodeConfigViewEvent());
        }
        public ICommand GoHistoryCMDViewCMD
        {
            get { return GetPropertyValue(() => GoHistoryCMDViewCMD); }
            set { SetPropertyValue(() => GoHistoryCMDViewCMD, value); }
        }


        public void CanExecuteGoHistoryCMDViewCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoHistoryCMDViewCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            
            EventAggregator.PublishMessage(new GoHistoryDataPageBaseNavigationRequest());
        }
        public ICommand GoPingViewCMD
        {
            get { return GetPropertyValue(() => GoPingViewCMD); }
            set { SetPropertyValue(() => GoPingViewCMD, value); }
        }


        public void CanExecuteGoPingViewCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoPingViewCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            
            EventAggregator.PublishMessage(new GoPingTestViewEvent());
        }
        public ICommand GoDebugViewCMD
        {
            get { return GetPropertyValue(() => GoDebugViewCMD); }
            set { SetPropertyValue(() => GoDebugViewCMD, value); }
        }


        public void CanExecuteGoDebugViewCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoDebugViewCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            
            EventAggregator.PublishMessage(new GoDebugViewPage());
        }
        public ICommand GoGetInfoViewCMD
        {
            get { return GetPropertyValue(() => GoGetInfoViewCMD); }
            set { SetPropertyValue(() => GoGetInfoViewCMD, value); }
        }


        public void CanExecuteGoGetInfoViewCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoGetInfoViewCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            
            EventAggregator.PublishMessage(new GoGetNodeStatusViewEvent());
        }

        public ICommand GoADViewCMD
        {
            get { return GetPropertyValue(() => GoADViewCMD); }
            set { SetPropertyValue(() => GoADViewCMD, value); }
        }


        public void CanExecuteGoADViewCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoADViewCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            
            EventAggregator.PublishMessage(new GoADViewEvent());
        }
    }
}