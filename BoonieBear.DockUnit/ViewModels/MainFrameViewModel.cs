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
            //绑定属性初始化
            AddPropertyChangedNotification(()=>IsShowBottomBar);
            IsShowBottomBar = Visibility.Hidden;
            AddPropertyChangedNotification(() => IsShowTopBar);
            IsShowTopBar = Visibility.Hidden;
            
        }
        #region 绑定属性
        public Visibility IsShowBottomBar
        {
            get { return GetPropertyValue(() => IsShowBottomBar); }
            set { SetPropertyValue(() => IsShowBottomBar, value); }
        }

        public Visibility IsShowTopBar
        {
            get { return GetPropertyValue(() => IsShowTopBar); }
            set { SetPropertyValue(() => IsShowTopBar, value); }
        }
        #endregion

        #region GoBack Command


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
            IsShowBottomBar =Visibility.Hidden;
            IsShowTopBar = Visibility.Hidden;
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }

        #endregion
    }
   
}
