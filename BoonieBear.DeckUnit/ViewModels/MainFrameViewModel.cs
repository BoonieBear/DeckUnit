using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.Events;
using BoonieBear.DeckUnit.Helps;
using BoonieBear.DeckUnit.JsonUtils;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.Frames;
using TinyMetroWpfLibrary.ViewModel;
using TinyMetroWpfLibrary.EventAggregation;
using BoonieBear.DeckUnit.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
namespace BoonieBear.DeckUnit.ViewModels
{
    /// <summary>
    ///程序主框架viewmodel，用于处理主框架消息
    /// </summary>
    public class MainFrameViewModel : MainWindowViewModelBase, IHandleMessage<StatusNotify>
    {
        public static MainFrameViewModel pMainFrame;
        private IDialogCoordinator _dialogCoordinator;

        
        public override void Initialize()
        {
            base.Initialize();
            TraceCollMt = new ObservableCollection<string>();

            DataCollMt = new ObservableCollection<CommandLog>();
           
            SwapMode = RegisterCommand(ExecuteSwapMode, CanExecuteSwapMode, true);
            SendCMD = RegisterCommand(ExecuteSendCMD, CanExecuteSendCMD, true);
            pMainFrame = this;
            //绑定属性初始化
            AddPropertyChangedNotification(() => StatusHeader);
            AddPropertyChangedNotification(()=>StatusDescription);
            AddPropertyChangedNotification(() => ModeType);
            IsStartPage= Visibility.Visible;
            StatusHeader = "甲板单元";
            StatusDescription = "正在运行";
            Level = NotifyLevel.Info;
            ModeType = true;
            DataRecvTime = "数据接收时间：---";

        }
        #region 绑定属性

        public ObservableCollection<string> TraceCollMt
        {
            get { return GetPropertyValue(() => TraceCollMt); }
            set { SetPropertyValue(() => TraceCollMt, value); }
        }

        public ObservableCollection<CommandLog> DataCollMt
        {
            get { return GetPropertyValue(() => DataCollMt); }
            set { SetPropertyValue(() => DataCollMt, value); }
        }
        public string DataRecvTime
        {
            get { return GetPropertyValue(() => DataRecvTime); }
            set { SetPropertyValue(() => DataRecvTime, value); }
        }
        public IDialogCoordinator DialogCoordinator
        {
            get { return _dialogCoordinator; }
            set { _dialogCoordinator = value; }
        }
        
        public string StatusHeader
        {
            get { return GetPropertyValue(() => StatusHeader); }
            set { SetPropertyValue(() => StatusHeader, value); }
        }
        public string StatusDescription
        {
            get { return GetPropertyValue(() => StatusDescription); }
            set { SetPropertyValue(() => StatusDescription, value); }
        }
        public NotifyLevel Level
        {
            get { return GetPropertyValue(() => Level); }
            set { SetPropertyValue(() => Level, value); }
        }
        //串口or网络选择 true：网络，false：串口
        public  bool ModeType
        {
            get { return GetPropertyValue(() => ModeType); }
            set
            {
                SetPropertyValue(() => ModeType, value);
            }
        }
        public string NetInput
        {
            get { return GetPropertyValue(() => NetInput); }
            set { SetPropertyValue(() => NetInput, value); }
        }
        public string CommInput
        {
            get { return GetPropertyValue(() => CommInput); }
            set { SetPropertyValue(() => CommInput, value); }
        }

        public string FilterString//空格或者不填表示没有过滤项
        {
            get { return GetPropertyValue(() => FilterString); }
            set { SetPropertyValue(() => FilterString, value); }
        }
        public string Filterlayer//
        {
            get { return GetPropertyValue(() => Filterlayer); }
            set { SetPropertyValue(() => Filterlayer, value); }
        }
        public int RecvMessage
        {
            get { return GetPropertyValue(() => RecvMessage); }
            set { SetPropertyValue(() => RecvMessage, value); }
        }
        public string Shellstring
        {
            get { return GetPropertyValue(() => Shellstring); }
            set { SetPropertyValue(() => Shellstring, value); }
        }
        public string Serialstring
        {
            get {return GetPropertyValue(() => Serialstring); }
            set { SetPropertyValue(() => Serialstring, value); }
        }
        public bool LoaderMode
        {
            get { return GetPropertyValue(() => LoaderMode); }
            set { SetPropertyValue(() => LoaderMode, value); }
        }
        public Visibility IsStartPage
        {
            get { return GetPropertyValue(() => IsStartPage); }
            set { SetPropertyValue(() => IsStartPage, value); }
        }
        #endregion


        #region SwapMode CMD
        public ICommand SwapMode
        {
            get { return GetPropertyValue(() => SwapMode); }
            set { SetPropertyValue(() => SwapMode, value); }
        }


        private void CanExecuteSwapMode(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private void ExecuteSwapMode(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            ModeType = !ModeType;
        }
        #endregion

        #region SendCMD
        public ICommand SendCMD
        {
            get { return GetPropertyValue(() => SendCMD); }
            set { SetPropertyValue(() => SendCMD, value); }
        }

        


        private void CanExecuteSendCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private async void ExecuteSendCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if (ModeType)
            {
                await UnitCore.Instance.NetEngine.SendConsoleCMD(NetInput);
            }
            else
            {
              //  await UnitCore.Instance.CommEngine.SendConsoleCMD(CommInput);//没有这个应用

            }
        }
        #endregion

        #region 消息响应
        public void Handle(StatusNotify message)
        {
            if (message != null)
            {
                StatusHeader = message.Source;
                StatusDescription = message.Msg;
                Level = message.Level;
            }
        }
        #endregion



        
    }
   
}
