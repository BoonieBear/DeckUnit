using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.DAL;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;
using BoonieBear.DeckUnit.Events;
using BoonieBear.DeckUnit.BaseType;
namespace BoonieBear.DeckUnit.ViewModels
{
    public class DownLoadedViewModel:ViewModelBase
    {
        public override void Initialize()
        {
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            FetchingData = RegisterCommand(ExecuteFetchingData, CanExecuteFetchingData, true);
            ListCollMt = new List<BDTask>();
            IsFetching = false;
            SelectedFromDate = DateTime.Today;
            SelectedToDate = DateTime.Today;
            RefreshVisble = Visibility.Visible;
        }

        public override void InitializePage(object extraData)
        {
        }
        public DateTime SelectedFromDate
        {
            get { return GetPropertyValue(() => SelectedFromDate); }
            set { SetPropertyValue(() => SelectedFromDate, value); }
        }

        public DateTime SelectedToDate
        {
            get { return GetPropertyValue(() => SelectedToDate); }
            set { SetPropertyValue(() => SelectedToDate, value); }
        }
        public Visibility RefreshVisble
        {
            get { return GetPropertyValue(() => RefreshVisble); }
            set { SetPropertyValue(() => RefreshVisble, value); }
        }
        public List<BDTask> ListCollMt
        {
            get { return GetPropertyValue(() => ListCollMt); }
            set { SetPropertyValue(() => ListCollMt, value); }
        }
        public BDTask SelectedBdTask
        {
            get { return GetPropertyValue(() => SelectedBdTask); }
            set
            {
                SetPropertyValue(() => SelectedBdTask, value);
                if (SelectedBdTask != null)
                {
                    EventAggregator.PublishMessage(new GoDownLoadingViewEvent(SelectedBdTask));
                }
            }
        }
        public bool IsFetching
        {
            get { return GetPropertyValue(() => IsFetching); }
            set { SetPropertyValue(() => IsFetching, value); }
        }
        public ICommand GoBackCommand
        {
            get { return GetPropertyValue(() => GoBackCommand); }
            set { SetPropertyValue(() => GoBackCommand, value); }
        }


        private void CanExecuteGoBackCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = !IsFetching;
        }


        private void ExecuteGoBackCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoHomePageNavigationEvent());
        }
        public ICommand FetchingData
        {
            get { return GetPropertyValue(() => FetchingData); }
            set { SetPropertyValue(() => FetchingData, value); }
        }


        public void CanExecuteFetchingData(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = UnitCore.Instance.UnitTraceService.IsOK;
        }


        public void ExecuteFetchingData(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            IsFetching = true;
            RefreshVisble = Visibility.Collapsed;
            //fetch data...

            var from = new DateTime(SelectedFromDate.Year, SelectedFromDate.Month, SelectedFromDate.Day);
            var to = new DateTime(SelectedToDate.Year, SelectedToDate.Month, SelectedToDate.Day);
            var lst = UnitCore.Instance.UnitTraceService.GetTaskList(from, to);
            if (lst != null)
            {
                ListCollMt.Clear();
                foreach (var task in lst)
                {
                    ListCollMt.Add(task);
                }
            }
            
            IsFetching = false;
            RefreshVisble = Visibility.Visible;
        }
    }
}
