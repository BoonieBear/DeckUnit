using System;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using BoonieBear.DeckUnit.MessageEvents;
using BoonieBear.TinyMetro.WPF.Events;
using BoonieBear.TinyMetro.WPF.ViewModel;

namespace BoonieBear.DeckUnit.ViewModels
{

    public class MainPageViewModel : ViewModelBase
    {
        #region Overrides of ViewModelBase


        public override void Initialize()
        {
            GoWaterTelPageCommand = RegisterCommand(ExecuteGoWaterTelPageCommand, CanExecuteGoWaterTelPageCommand, true);
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

        #region GoWaterTelPageCommand 


        public ICommand GoWaterTelPageCommand
        {
            get { return GetPropertyValue(() => GoWaterTelPageCommand); }
            set { SetPropertyValue(() => GoWaterTelPageCommand, value); }
        }


        public void CanExecuteGoWaterTelPageCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoWaterTelPageCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            
            EventAggregator.PublishMessage(new GoWaterTelPageBaseNavigationRequest());
        }


        #endregion
    }
}