﻿using System.Collections.ObjectModel;
using System.Resources;
using System.Windows.Input;
using BoonieBear.DeckUnit.Helps;
using BoonieBear.TinyMetro.WPF.Events;
using BoonieBear.TinyMetro.WPF.ViewModel;
using BoonieBear.DeckUnit.SysResourceLib;
namespace BoonieBear.DeckUnit.ViewModels
{
    public class SystemResourceViewModel : ViewModelBase
    {
        #region Overrides of ViewModelBase


        public override void Initialize()
        {
            GoPage1Command = RegisterCommand(ExecuteGoPage1Command, CanExecuteGoPage1Command, true);
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
        
        }


        public override void InitializePage(object extraData)
        {
            RefreshInfos();
        }

        private void RefreshInfos()
        {
            var ir = GetSystemInfo.CreateResources();
            if (MemInfos == null)
                MemInfos = new ObservableCollection<SystemInfo>();
            MemInfos.Clear();
           
            MemInfos.Add(new SystemInfo() { Category = "内存使用", Number = ir.GetMemoryUsage() });
            
            //MemInfos.Add(new SystemInfo() { Category = "内存使用1", Number = ir.GetMemoryUsage() });
            MemInfos.Add(new SystemInfo() { Category = "磁盘空间使用", Number = ir.GetDiskUsage() });
        }
        #endregion

        #region 绑定属性
        
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


        public void ExecuteGoBackCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }

        #endregion

        #region GoPage1 Command


        public ICommand GoPage1Command
        {
            get { return GetPropertyValue(() => GoPage1Command); }
            set { SetPropertyValue(() => GoPage1Command, value); }
        }


        public void CanExecuteGoPage1Command(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoPage1Command(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            
        }

        #endregion
    }
}