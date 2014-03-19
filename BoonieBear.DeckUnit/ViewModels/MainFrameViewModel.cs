using System.Windows;
using System.Windows.Input;
using BoonieBear.TinyMetro.WPF.Events;
using BoonieBear.TinyMetro.WPF.Frames;
using BoonieBear.TinyMetro.WPF.ViewModel;

namespace BoonieBear.DeckUnit.ViewModels
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
			ShowOptionPanelCommand = RegisterCommand(ExecuteShowOptionPanelCommand, CanExecuteShowOptionPanelCommand, true);
            pMainFrame = this;
            //绑定属性初始化
            AddPropertyChangedNotification(()=>IsShowBottomBar);
            IsShowBottomBar = Visibility.Hidden;
			AddPropertyChangedNotification(()=>OptionPanelWidth);
            OptionPanelWidth = 0;

            
        }
        #region 绑定属性
        public Visibility IsShowBottomBar
        {
            get { return GetPropertyValue(() => IsShowBottomBar); }
            set { SetPropertyValue(() => IsShowBottomBar, value); }
        }
        public int OptionPanelWidth
        {
            get { return GetPropertyValue(() => OptionPanelWidth); }
            set { SetPropertyValue(() => OptionPanelWidth, value); }
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
            EventAggregator.PublishMessage(new ChangeAnimationModeRequest(AnimationMode.SmoothSlide));
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
            EventAggregator.PublishMessage(new ChangeAnimationModeRequest(AnimationMode.Fade));
        }

        #endregion
		
		#region ShowOptionPanelCommand
		public ICommand ShowOptionPanelCommand
		{
			get { return GetPropertyValue(() => ShowOptionPanelCommand); }
            set { SetPropertyValue(() => ShowOptionPanelCommand, value); }
		}
		private void CanExecuteShowOptionPanelCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }
		private void ExecuteShowOptionPanelCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
			if(OptionPanelWidth==0)
				OptionPanelWidth = 120;
			else
				OptionPanelWidth=0;
		}
		#endregion
    }
   
}
