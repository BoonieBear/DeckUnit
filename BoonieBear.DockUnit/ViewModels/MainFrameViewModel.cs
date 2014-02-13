using System.Windows;
using System.Windows.Input;
using BoonieBear.TinyMetro.WPF.Events;
using BoonieBear.TinyMetro.WPF.ViewModel;

namespace BoonieBear.DockUnit.ViewModels
{
    /// <summary>
    ///程序主框架viewmodel，用于处理主框架消息
    /// </summary>
    public class MainFrameViewModel : MainWindowViewModelBase
    {
        public static MainFrameViewModel pMainFrame;
        public override void Initialize()
        {
            base.Initialize();
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            pMainFrame = this;
            AddPropertyChangedNotification(()=>IsShowBottomBar);
            IsShowBottomBar = Visibility.Hidden;
            
        }

        public void ShowBottomBar(Visibility v)
        {
            IsShowBottomBar = v;
        }
        public Visibility IsShowBottomBar
        {
            get { return GetPropertyValue(() => IsShowBottomBar); }
            set { SetPropertyValue(() => IsShowBottomBar, value); }
        }

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
            ShowBottomBar(Visibility.Hidden);
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }

        #endregion
    }
   
}
