using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;
namespace BoonieBear.DeckUnit.Mov4500UI.ViewModel
{
    public class HomePageViewModel:ViewModelBase
    {

        public override void Initialize()
        {
            RecvMsgNumber = 0;
            StartLiveCapture = RegisterCommand(ExecuteStartLiveCapture, CanExecuteStartLiveCapture, true);
        }
        public override void InitializePage(object extraData)
        {
        }
        public int RecvMsgNumber
        {
            get { return GetPropertyValue(() => RecvMsgNumber); }
            set { SetPropertyValue(() => RecvMsgNumber, value); }
        }
        public ICommand StartLiveCapture
        {
            get { return GetPropertyValue(() => StartLiveCapture); }
            set { SetPropertyValue(() => StartLiveCapture, value); }
        }


        public void CanExecuteStartLiveCapture(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteStartLiveCapture(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoLiveCaptureNavigationRequest());
        }
    }
}
