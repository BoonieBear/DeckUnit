using System.Windows.Input;
using BoonieBear.DeckUnit.Core;
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
            SelectNode = RegisterCommand(ExecuteSelectNode, CanExecuteSelectNode, true);
            SelectDevice = RegisterCommand(ExecuteSelectDevice, CanExecuteSelectDevice, true);
            SelectNet = RegisterCommand(ExecuteSelectNet, CanExecuteSelectNet, true);
            GoGetDeviceDataPage = RegisterCommand(ExecuteGoGetDeviceDataPage, CanExecuteGoGetDeviceDataPage, true);
        }


        public override void InitializePage(object extraData)
        {
            IsNodeActive = true;
            IsDeviceActive = false;
            IsNetActive = false;

        }

        #endregion

        #region 绑定属性
        public bool IsNodeActive
        {
            get { return GetPropertyValue(() => IsNodeActive); }
            set
            {
                SetPropertyValue(() => IsNodeActive, value);
                if (value==true)
                    SelectIndex = 0;
            }
        }
        public bool IsDeviceActive
        {
            get { return GetPropertyValue(() => IsDeviceActive); }
            set
            {
                SetPropertyValue(() => IsDeviceActive, value);
                if (value == true)
                    SelectIndex = 1;
            }
        }
        public bool IsNetActive
        {
            get { return GetPropertyValue(() => IsNetActive); }
            set
            {
                SetPropertyValue(() => IsNetActive, value);
                if (value == true)
                    SelectIndex = 2;
            }
        }
        public int SelectIndex
        {
            get { return GetPropertyValue(() => SelectIndex); }
            set { SetPropertyValue(() => SelectIndex, value); }
        }
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

        #region SelectNode
        public ICommand SelectNode
        {
            get { return GetPropertyValue(() => SelectNode); }
            set { SetPropertyValue(() => SelectNode, value); }
        }


        public void CanExecuteSelectNode(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteSelectNode(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            IsNodeActive = true;
            IsDeviceActive = false;
            IsNetActive = false;
        }
        #endregion

        #region node cmd

        #endregion
        #region device cmd

        public ICommand GoGetDeviceDataPage
        {
            get { return GetPropertyValue(() => GoGetDeviceDataPage); }
            set { SetPropertyValue(() => GoGetDeviceDataPage, value); }
        }


        public void CanExecuteGoGetDeviceDataPage(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public  void ExecuteGoGetDeviceDataPage(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoSimplePage("获取设备数据"));
        }
        #endregion
        #region network
        #endregion
        #region SelectDevice
        public ICommand SelectDevice
        {
            get { return GetPropertyValue(() => SelectDevice); }
            set { SetPropertyValue(() => SelectDevice, value); }
        }


        public void CanExecuteSelectDevice(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteSelectDevice(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            IsNodeActive = false;
            IsDeviceActive = true;
            IsNetActive = false;
        }
        #endregion

        #region SelectNet
        public ICommand SelectNet
        {
            get { return GetPropertyValue(() => SelectNet); }
            set { SetPropertyValue(() => SelectNet, value); }
        }


        public void CanExecuteSelectNet(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteSelectNet(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            IsNodeActive = false;
            IsDeviceActive = false;
            IsNetActive = true;
        }
        #endregion
    }
}
