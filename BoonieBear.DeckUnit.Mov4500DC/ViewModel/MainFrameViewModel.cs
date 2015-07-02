using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using BoonieBear.DeckUnit.Mov4500UI.Models;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.Frames;
using TinyMetroWpfLibrary.ViewModel;

namespace BoonieBear.DeckUnit.Mov4500UI.ViewModel
{
    public class MainFrameViewModel : MainWindowViewModelBase
    {
        public static MainFrameViewModel pMainFrame { get; set; }
        public override void Initialize()
        {
            base.Initialize();
            pMainFrame = this;
        }

        #region action
        internal void GoToGlobalSettings()
        {
            throw new NotImplementedException();
        }
        internal void GoBack()
        {
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }
        internal void GoCommandWin()
        {
            throw new NotImplementedException();
        }
        internal void GoHome()
        {
            throw new NotImplementedException();
        }
        internal void ExitProgram()
        {
            Application.Current.Shutdown();
        }

        internal void ShowAbout()
        {
            AboutVisibility = Visibility.Visible;
        }

        internal void RefreshStatus()
        {
            NetworkStatus = Status.NetworkStatus;
            LastUpdateTime = Status.LastUpdateTime;
            ReceivebinaryDataCount = Status.ReceivebinaryDataCount.ToString();

        }

        #endregion
        #region binding property
        public Visibility AboutVisibility
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

        

        public string LastUpdateTime
        {
            get { return GetPropertyValue(() => LastUpdateTime); }
            set { SetPropertyValue(() => LastUpdateTime, value); }
        }

        public string ReceivebinaryDataCount
        {
            get { return GetPropertyValue(() => ReceivebinaryDataCount); }
            set { SetPropertyValue(() => ReceivebinaryDataCount, value); }
        }

        
        #endregion

    }
}
