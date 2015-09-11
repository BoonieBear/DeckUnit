using System;
using System.Windows.Input;
using BoonieBear.DeckUnit.ACNP;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.Events;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System.Threading;
using System.Threading.Tasks;
using BoonieBear.DeckUnit.BaseType;
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
            GoGetDeviceDataPage = RegisterCommand(ExecuteGoGetDeviceDataPage, CanExecuteGoGetDeviceDataPage, true);
            GoGetDeviceStatusPage = RegisterCommand(ExecuteGoGetDeviceStatusPage, CanExecuteGoGetDeviceStatusPage, true);
            GoGetNodeStatusPage = RegisterCommand(ExecuteGoGetNodeStatusPage, CanExecuteGoGetNodeStatusPage, true);
            GoGetNeiborListPage = RegisterCommand(ExecuteGoGetNeiborListPage, CanExecuteGoGetNeiborListPage, true);
            GoGetNetListPage = RegisterCommand(ExecuteGoGetNetListPage, CanExecuteGoGetNetListPage, true);
            GoGetNetSimpleListPage = RegisterCommand(ExecuteGoGetNetSimpleListPage, CanExecuteGoGetNetSimpleListPage,
                true);
            GoGetNodeInfoPage = RegisterCommand(ExecuteGoGetNodeInfoPage, CanExecuteGoGetNodeInfoPage, true);
            GoGetNodeInfoListPage = RegisterCommand(ExecuteGoGetNodeInfoListPage, CanExecuteGoGetNodeInfoListPage, true);
            GoGetNodeRoutePage = RegisterCommand(ExecuteGoGetNodeRoutePage, CanExecuteGoGetNodeRoutePage, true);
            ResetNetwork = RegisterCommand(ExecuteResetNetwork, CanExecuteResetNetwork, true);
            GoDeviceBackSet = RegisterCommand(ExecuteGoDeviceBackSet, CanExecuteGoDeviceBackSet, true);
            GoDeviceParaSet = RegisterCommand(ExecuteGoDeviceParaSet, CanExecuteGoDeviceParaSet, true);
            GoSetRecvEmitPage = RegisterCommand(ExecuteGoSetRecvEmitPage, CanExecuteGoSetRecvEmitPage, true);
            GoComSchemaPage = RegisterCommand(ExecuteGoComSchemaPage, CanExecuteGoComSchemaPage, true);
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
                if (value == true)
                {
                    IsDeviceActive = false;
                    IsNetActive = false;
                    SelectIndex = 0;
                }
                else
                {
                    if (IsDeviceActive == false && IsNetActive==false)
                        IsNodeActive = true;
                }
            }
        }
        public bool IsDeviceActive
        {
            get { return GetPropertyValue(() => IsDeviceActive); }
            set
            {
                SetPropertyValue(() => IsDeviceActive, value);
                if (value == true)
                {
                    IsNodeActive = false;
                    IsNetActive = false;
                    SelectIndex = 1;
                }
                else
                {
                    if (IsNodeActive == false && IsNetActive == false)
                        IsDeviceActive = true;
                }
            }
        }
        public bool IsNetActive
        {
            get { return GetPropertyValue(() => IsNetActive); }
            set
            {
                SetPropertyValue(() => IsNetActive, value);
                if (value == true)
                {
                    IsNodeActive = false;
                    IsDeviceActive = false;
                    SelectIndex = 2;
                }
                else
                {
                    if (IsNodeActive == false && IsDeviceActive == false)
                        IsNetActive = true;
                }
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

       

        #region node cmd
        public ICommand GoGetNodeStatusPage
        {
            get { return GetPropertyValue(() => GoGetNodeStatusPage); }
            set { SetPropertyValue(() => GoGetNodeStatusPage, value); }
        }


        public void CanExecuteGoGetNodeStatusPage(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoGetNodeStatusPage(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoSimplePage("获取节点状态"));
        }
        public ICommand GoGetNodeInfoPage
        {
            get { return GetPropertyValue(() => GoGetNodeInfoPage); }
            set { SetPropertyValue(() => GoGetNodeInfoPage, value); }
        }


        public void CanExecuteGoGetNodeInfoPage(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoGetNodeInfoPage(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoSimplePage("获取节点信息"));
        }
        public ICommand GoGetNodeInfoListPage
        {
            get { return GetPropertyValue(() => GoGetNodeInfoListPage); }
            set { SetPropertyValue(() => GoGetNodeInfoListPage, value); }
        }


        public void CanExecuteGoGetNodeInfoListPage(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoGetNodeInfoListPage(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoSimplePage("获取节点信息表"));
        }
        public ICommand GoGetNodeRoutePage
        {
            get { return GetPropertyValue(() => GoGetNodeRoutePage); }
            set { SetPropertyValue(() => GoGetNodeRoutePage, value); }
        }


        public void CanExecuteGoGetNodeRoutePage(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoGetNodeRoutePage(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoSimplePage("获取节点路由"));
        }
        public ICommand GoSetRecvEmitPage
        {
            get { return GetPropertyValue(() => GoSetRecvEmitPage); }
            set { SetPropertyValue(() => GoSetRecvEmitPage, value); }
        }


        public void CanExecuteGoSetRecvEmitPage(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoSetRecvEmitPage(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoNodeRecvEmitSetPage());
        }
        public ICommand GoComSchemaPage
        {
            get { return GetPropertyValue(() => GoComSchemaPage); }
            set { SetPropertyValue(() => GoComSchemaPage, value); }
        }


        public void CanExecuteGoComSchemaPage(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoComSchemaPage(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoComSchemaEvent());
        }
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

        public ICommand GoGetDeviceStatusPage
        {
            get { return GetPropertyValue(() => GoGetDeviceStatusPage); }
            set { SetPropertyValue(() => GoGetDeviceStatusPage, value); }
        }


        public void CanExecuteGoGetDeviceStatusPage(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoGetDeviceStatusPage(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoSimplePage("获取设备状态"));
        }

        


        public ICommand GoDeviceBackSet
        {
            get { return GetPropertyValue(() => GoDeviceBackSet); }
            set { SetPropertyValue(() => GoDeviceBackSet, value); }
        }


        public void CanExecuteGoDeviceBackSet(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoDeviceBackSet(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoDeviceBackSetPage());
        }
        public ICommand GoDeviceParaSet
        {
            get { return GetPropertyValue(() => GoDeviceParaSet); }
            set { SetPropertyValue(() => GoDeviceParaSet, value); }
        }


        public void CanExecuteGoDeviceParaSet(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoDeviceParaSet(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoDeviceParaSetPage());
        }
        #endregion

        #region network
        public ICommand GoGetNeiborListPage
        {
            get { return GetPropertyValue(() => GoGetNeiborListPage); }
            set { SetPropertyValue(() => GoGetNeiborListPage, value); }
        }


        public void CanExecuteGoGetNeiborListPage(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoGetNeiborListPage(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoSimplePage("获取邻接点表"));
        }

        public ICommand GoGetNetListPage
        {
            get { return GetPropertyValue(() => GoGetNetListPage); }
            set { SetPropertyValue(() => GoGetNetListPage, value); }
        }


        public void CanExecuteGoGetNetListPage(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoGetNetListPage(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoSimplePage("获取网络表"));
        }
        public ICommand GoGetNetSimpleListPage
        {
            get { return GetPropertyValue(() => GoGetNetSimpleListPage); }
            set { SetPropertyValue(() => GoGetNetSimpleListPage, value); }
        }


        public void CanExecuteGoGetNetSimpleListPage(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoGetNetSimpleListPage(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoSimplePage("获取网络简表"));
        }



        public ICommand ResetNetwork
        {
            get { return GetPropertyValue(() => ResetNetwork); }
            set { SetPropertyValue(() => ResetNetwork, value); }
        }


        public void CanExecuteResetNetwork(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public async void ExecuteResetNetwork(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "确定",
                NegativeButtonText = "取消",
                AnimateShow = true,
                AnimateHide = false
            };

            var result = await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "网络命令",
               "确定要全网复位吗？",
               MessageDialogStyle.AffirmativeAndNegative, mySettings);



            if (result == MessageDialogResult.Affirmative)
            {
                ACNBuilder.Pack200();
                var cmd = ACNProtocol.Package(false);
                var cl = new CommandLog();
                cl.DestID = 0;
                cl.SourceID = (int)ACNProtocol.SourceID;
                cl.LogTime = DateTime.Now;
                cl.CommID = 200;
                cl.Type = false;
                UnitCore.Instance.UnitTraceService.Save(cl, cmd);
                await UnitCore.Instance.NetEngine.SendCMD(cmd);
            }
        }
        #endregion


    }
}
