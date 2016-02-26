using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using BoonieBear.DeckUnit.Mov4500UI.Core;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using BoonieBear.DeckUnit.Mov4500UI.Models;
using BoonieBear.DeckUnit.Mov4500UI.Views;
using DevExpress.Data.Async.Helpers;
using TinyMetroWpfLibrary.Controller;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.Frames;
using TinyMetroWpfLibrary.ViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace BoonieBear.DeckUnit.Mov4500UI.ViewModel
{
    public class MainFrameViewModel : MainWindowViewModelBase
    {
        public static MainFrameViewModel pMainFrame { get; set; }
        private IDialogCoordinator _dialogCoordinator { get; set; }
        private DispatcherTimer t = null;
        public override void Initialize()
        {
            base.Initialize();
            pMainFrame = this;
            MsgLog = new ObservableCollection<string>();
            MsgLog.CollectionChanged+=MsgLog_CollectionChanged;
            t = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, RefreshStatus, Dispatcher.CurrentDispatcher);
            Version = Properties.Resources.Version_Number;
            BuildNo = Properties.Resources.Build_Number;
        }

        private void RefreshStatus(object sender, EventArgs e)
        {
            NetworkStatus = Status.NetworkStatus;
            LastUpdateTime = Status.LastUpdateTime;
            ReceiveMsgCount = Status.ReceiveMsgCount.ToString();
        }

        private void MsgLog_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnPropertyChanged(() => MsgLog);
            var bar = (ScrollViewer)((MainFrame)App.Current.MainWindow).scrollbar;
            if (bar != null)
                bar.ScrollToEnd();
        }

        #region action
        internal void GoToGlobalSettings()
        {
            EventAggregator.PublishMessage(new GoSettingNavigation());
        }
        internal void GoBack()
        {
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }
        internal void GoCommandWin()
        {
            
        }
        internal void ExitProgram()
        {
            Application.Current.Shutdown();
        }

        internal void ShowAbout()
        {
            AboutVisibility = true;
        }

        #endregion
        #region binding property
        public ObservableCollection<string> MsgLog
        {
            get { return GetPropertyValue(() => MsgLog); }
            set { SetPropertyValue(() => MsgLog, value); }
        }
        public IDialogCoordinator DialogCoordinator
        {
            get { return _dialogCoordinator; }
            set { _dialogCoordinator = value; }
        }
        public bool AboutVisibility
        {
            get { return GetPropertyValue(() => AboutVisibility); }
            set { SetPropertyValue(() => AboutVisibility, value); }
        }
        public string BuildNo
        {
            get { return GetPropertyValue(() => BuildNo); }
            set { SetPropertyValue(() => BuildNo, value); }
        }

        public string Version
        {
            get { return GetPropertyValue(() => Version); }
            set { SetPropertyValue(() => Version, value); }
        }

        public string NetworkStatus
        {
            get { return GetPropertyValue(() => NetworkStatus); }
            set { SetPropertyValue(() => NetworkStatus, value); }
        }
        public string CommStatus
        {
            get { return GetPropertyValue(() => CommStatus); }
            set { SetPropertyValue(() => CommStatus, value); }
        }
        

        public string LastUpdateTime
        {
            get { return GetPropertyValue(() => LastUpdateTime); }
            set { SetPropertyValue(() => LastUpdateTime, value); }
        }

        public string ReceiveMsgCount
        {
            get { return GetPropertyValue(() => ReceiveMsgCount); }
            set { SetPropertyValue(() => ReceiveMsgCount, value); }
        }

        
        #endregion

    }
}
