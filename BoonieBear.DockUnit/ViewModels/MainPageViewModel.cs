using System;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using BoonieBear.DockUnit.NavigationEvents;
using BoonieBear.TinyMetro.WPF.Events;
using BoonieBear.TinyMetro.WPF.ViewModel;

namespace BoonieBear.DockUnit.ViewModels
{

    public class MainPageViewModel : ViewModelBase
    {
        #region Overrides of ViewModelBase


        public override void Initialize()
        {
            GoPage1Command = RegisterCommand(ExecuteGoPage1Command, CanExecuteGoPage1Command, true);
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

        #region GoPage1 Command


        public ICommand GoPage1Command
        {
            get { return GetPropertyValue(() => GoPage1Command); }
            set { SetPropertyValue(() => GoPage1Command, value); }
        }


        public void CanExecuteGoPage1Command(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoPage1Command(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoPage1NavigationRequest());
        }

        #endregion
    }
}