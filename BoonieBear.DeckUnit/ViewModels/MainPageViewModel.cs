using System;
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
    }
}