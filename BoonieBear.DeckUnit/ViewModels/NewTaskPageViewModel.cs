using System.Windows;
using System.Windows.Input;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;

namespace BoonieBear.DeckUnit.ViewModels
{
    public class NewTaskPageViewModel : ViewModelBase
    {
        public override void Initialize()
        {
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            CommIndex = 0;
            DestID = 1;
            TypeIndex = 0;
        }


        public override void InitializePage(object extraData)
        {
        }


        #region 绑定属性
        public int DestID
        {
            get { return GetPropertyValue(() => DestID); }
            set { SetPropertyValue(() => DestID, value); }
        }
        public int CommIndex
        {
            get { return GetPropertyValue(() => CommIndex); }
            set { SetPropertyValue(() => CommIndex, value); }
        }
        public int TypeIndex
        {
            get { return GetPropertyValue(() => TypeIndex); }
            set
            {
                SetPropertyValue(() => TypeIndex, value);
                switch (TypeIndex)
                {
                    case 0:
                        ShowCondition1 = true;
                        break;
                    case 1:
                        ShowCondition2 = true;
                        break;
                    case 2:
                        ShowCondition3 = true;
                        break;
                    case 3:
                        ShowCondition4 = true;
                        break;
                    default:
                        break;
                }
            }
        }

        public bool ShowCondition1
        {
            get { return GetPropertyValue(() => ShowCondition1); }
            set { SetPropertyValue(() => ShowCondition1, value); }
        }

        public bool ShowCondition2
        {
            get { return GetPropertyValue(() => ShowCondition2); }
            set { SetPropertyValue(() => ShowCondition2, value); }
        }
        public bool ShowCondition3
        {
            get { return GetPropertyValue(() => ShowCondition3); }
            set { SetPropertyValue(() => ShowCondition3, value); }
        }
        public bool ShowCondition4
        {
            get { return GetPropertyValue(() => ShowCondition4); }
            set { SetPropertyValue(() => ShowCondition4, value); }
        }
        #endregion

        public ICommand GoBackCommand
        {
            get { return GetPropertyValue(() => GoBackCommand); }
            set { SetPropertyValue(() => GoBackCommand, value); }
        }


        private void CanExecuteGoBackCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private async void ExecuteGoBackCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }
    }
}
