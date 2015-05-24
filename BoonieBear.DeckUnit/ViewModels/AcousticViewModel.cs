using System.Windows.Input;
using BoonieBear.DeckUnit.Events;
using BoonieBear.TinyMetro.WPF.Events;
using BoonieBear.TinyMetro.WPF.ViewModel;

namespace BoonieBear.DeckUnit.ViewModels
{
    /// <summary>
    /// That's the ViewModel that belongs to the Example View
    /// </summary>
    public class AcousticViewModel : ViewModelBase
    {
        #region Overrides of ViewModelBase


        public override void Initialize()
        {
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            GoDataPageCommand = RegisterCommand(ExecuteGoDataPageCommand, CanExecuteGoDataPageCommand, true);
            
        }


        public override void InitializePage(object extraData)
        {
        }

        #endregion

        #region 绑定属性
        

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

        #region GoDataPageCommand
        public ICommand GoDataPageCommand
        {
            get { return GetPropertyValue(() => GoDataPageCommand); }
            set { SetPropertyValue(() => GoDataPageCommand, value); }
        }


        private void CanExecuteGoDataPageCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private void ExecuteGoDataPageCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {

            EventAggregator.PublishMessage(new GoHistoryDataPageBaseNavigationRequest("历史数据"));
        }
    #endregion

    }
}
