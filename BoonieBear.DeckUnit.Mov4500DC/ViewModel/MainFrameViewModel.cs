using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using BoonieBear.DeckUnit.Mov4500UI.Models;
using DevExpress.Data.Async.Helpers;
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

        public override void Initialize()
        {
            base.Initialize();
            pMainFrame = this;
            MsgLog = new ObservableCollection<string>();
            MsgLog.CollectionChanged+=MsgLog_CollectionChanged;
        }

        private void MsgLog_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(MsgLog.Count>200)
                MsgLog.RemoveAt(0);
            else
                base.OnPropertyChanged(() => MsgLog);
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

        internal void RefreshStatus()
        {
            NetworkStatus = Status.NetworkStatus;
            LastUpdateTime = Status.LastUpdateTime;
            ReceiveMsgCount = Status.ReceiveMsgCount.ToString();

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
