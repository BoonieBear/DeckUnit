using System.Windows.Input;
using BoonieBear.DeckUnit.Events;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;

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
            GoHomeCMD = RegisterCommand(ExecuteGoHomeCMD, CanExecuteGoHomeCMD, true);
           
        }


        public override void InitializePage(object extraData)
        {
        }

        #endregion

        #region 绑定属性
        

        #endregion

        #region GoHomeCMD Command


        public ICommand GoHomeCMD
        {
            get { return GetPropertyValue(() => GoHomeCMD); }
            set { SetPropertyValue(() => GoHomeCMD, value); }
        }


        public void CanExecuteGoHomeCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoHomeCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }

        #endregion

       
    }
}
