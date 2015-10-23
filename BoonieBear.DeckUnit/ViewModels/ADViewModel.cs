using BoonieBear.DeckUnit.Models;
using BoonieBear.DeckUnit.Resource;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;

namespace BoonieBear.DeckUnit.ViewModels
{
    public class ADViewModel : ViewModelBase
    {
        #region Overrides of ViewModelBase


        public override void Initialize()
        {
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            BeginAD = RegisterCommand(ExecuteBeginAD, CanExecuteBeginAD, true);
            StopAD = RegisterCommand(ExecuteStopAD, CanExecuteStopAD, true);
        }


        public override void InitializePage(object extraData)
        {
            IsWorking = false;
            TotalADByte = 0;
            RefreshInfos();
        }
        private void RefreshInfos()
        {
            var ir = GetSystemInfo.CreateResourcesProbe();
            if (MemInfos == null)
                MemInfos = new ObservableCollection<SystemInfo>();
            MemInfos.Clear();
            string MyExecPath = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            string diskname = MyExecPath.Substring(0, 2);
            double freesize = ir.GetDiskFree(diskname);//KB
            freesize /= 1024;//MB
            freesize /= 1024;//GB
            double totalsize = ir.GetDiskSize(diskname);//KB
            totalsize /= 1024;//MB
            totalsize /= 1024;//GB
            MemInfos.Add(new SystemInfo() { Name = "空闲", Size = Math.Round(freesize,2) });
            MemInfos.Add(new SystemInfo() { Name = "已使用", Size = Math.Round(totalsize - freesize,2) });
        }
        #endregion

        #region 绑定属性

        public bool IsWorking
        {
            get { return GetPropertyValue(() => IsWorking); }
            set { SetPropertyValue(() => IsWorking, value); }
        }
        public int TotalADByte
        {
            get { return GetPropertyValue(() => TotalADByte); }
            set { SetPropertyValue(() => TotalADByte, value); }
        }

        public ObservableCollection<SystemInfo> MemInfos
        {
            get { return GetPropertyValue(() => MemInfos); }
            set { SetPropertyValue(() => MemInfos, value); }
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


        public async void ExecuteGoBackCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if (IsWorking)
            {
                var md = new MetroDialogSettings();
                md.AffirmativeButtonText = "确定";
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "提示",
                "尚在接收AD数据中，请先停止AD采集再离开页面", MessageDialogStyle.Affirmative, md);
                return;
            }
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }

        public ICommand BeginAD
        {
            get { return GetPropertyValue(() => BeginAD); }
            set { SetPropertyValue(() => BeginAD, value); }
        }


        public void CanExecuteBeginAD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteBeginAD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }

        public ICommand StopAD
        {
            get { return GetPropertyValue(() => StopAD); }
            set { SetPropertyValue(() => StopAD, value); }
        }


        public void CanExecuteStopAD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteStopAD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }
        #endregion
    }
}
