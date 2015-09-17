using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.Helps;
using BoonieBear.DeckUnit.UBP;
using MahApps.Metro.Controls.Dialogs;
using TinyMetroWpfLibrary.EventAggregation;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;
using BoonieBear.DeckUnit.Events;
using TinyMetroWpfLibrary.Utility;
using BoonieBear.DeckUnit.BaseType;
namespace BoonieBear.DeckUnit.ViewModels
{
    public class DownLoadingViewModel : ViewModelBase, BoonieBear.DeckUnit.UBP.BDObserver<EventArgs>
    {
        private BDTask _currentBdTask;
        public override void Initialize()
        {
            CmdId = 1;
            DeckDataProtocol.AddCallBack(this);
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            GoDataCommand = RegisterCommand(ExecuteGoDataCommand, CanExecuteGoDataCommand, true);
            StartTaskCommand = RegisterCommand(ExecuteStartTaskCommand, CanExecuteStartTaskCommand, true);
            DeleteTaskCommand = RegisterCommand(ExecuteDeleteTaskCommand, CanExecuteDeleteTaskCommand, true);
            IsWorking = false;
            TaskState = "UNSTART";
        }


        public override void InitializePage(object extraData)
        {
            _currentBdTask = extraData as BDTask;
            if (_currentBdTask == null)
                return;
            TaskID = _currentBdTask.TaskID;
            DestID = _currentBdTask.DestID;
            CmdId = _currentBdTask.CommID;
            StarTime = _currentBdTask.StarTime;
        }


        #region 绑定属性
        public  Int64 TaskID
        {
            get { return GetPropertyValue(() => TaskID); }
            set { SetPropertyValue(() => TaskID, value); }
        }
        public  string TaskStatus
        {
            get { return GetPropertyValue(() => TaskStatus); }
            set { SetPropertyValue(() => TaskStatus, value); }
        }
        public  int DestID
        {
            get { return GetPropertyValue(() => DestID); }
            set { SetPropertyValue(() => DestID, value); }
        }
        public int CmdId
        {
            get { return CmdId; }
            set
            {
                CmdId = value;
                switch (CmdId)
                {
                    case 1:
                        CmdIDDesc = "@SP";
                        break;
                    case 2:
                        CmdIDDesc = "时间段索取-" + Encoding.Default.GetString(_currentBdTask.ParaBytes);
                        break;
                    case 3:
                        CmdIDDesc = "抽取索取-"+ Encoding.Default.GetString(_currentBdTask.ParaBytes);
                        break;
                    case 4:
                        CmdIDDesc = "ADCP继续工作";
                        break;
                }
            }
        }
        public string CmdIDDesc
        {
            get { return GetPropertyValue(() => CmdIDDesc); }
            set { SetPropertyValue(() => CmdIDDesc, value); }
        }
        public  double RetryRate
        {
            get { return GetPropertyValue(() => RetryRate); }
            set { SetPropertyValue(() => RetryRate, value); }
        }
        public  DateTime StarTime
        {
            get { return GetPropertyValue(() => StarTime); }
            set { SetPropertyValue(() => StarTime, value); }
        }
        public  DateTime LastTime
        {
            get { return GetPropertyValue(() => LastTime); }
            set { SetPropertyValue(() => LastTime, value); }
        }
        public  int TotalSeconds
        {
            get { return GetPropertyValue(() => TotalSeconds); }
            set { SetPropertyValue(() => TotalSeconds, value); }
        }
        public  int RecvBytes
        {
            get { return GetPropertyValue(() => RecvBytes); }
            set { SetPropertyValue(() => RecvBytes, value); }
        }
        public string TaskState//UNSTART, STOP, WORKING, COMPLETED
        {
            get { return GetPropertyValue(() => TaskState); }
            set { SetPropertyValue(() => TaskState, value); }
        }

        public bool IsWorking
        {
            get { return GetPropertyValue(() => IsWorking); }
            set { SetPropertyValue(() => IsWorking, value); }
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

                if(TaskState=="WORKING") //正在工作
                {
                    var md = new MetroDialogSettings();
                    md.AffirmativeButtonText = "好的";
                    await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "提示",
                    "请先停止任务再切换页面",MessageDialogStyle.Affirmative,md);
                    return;
                }
            
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }

        public ICommand GoDataCommand
        {
            get { return GetPropertyValue(() => GoDataCommand); }
            set { SetPropertyValue(() => GoDataCommand, value); }
        }


        private void CanExecuteGoDataCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
            if(_currentBdTask!=null)
                if(_currentBdTask.TaskStage!=(int) TaskStage.Finish)
                    eventArgs.CanExecute = false;
        }


        private  void ExecuteGoDataCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoADCPDataViewEvent(_currentBdTask));
        }
        public ICommand DeleteTaskCommand
        {
            get { return GetPropertyValue(() => DeleteTaskCommand); }
            set { SetPropertyValue(() => DeleteTaskCommand, value); }
        }


        private void CanExecuteDeleteTaskCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = false;
            if (TaskState=="STOP"||TaskState=="COMPLETED")
                eventArgs.CanExecute = true;
        }


        private async void ExecuteDeleteTaskCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if (_currentBdTask != null)
            {
                if (UnitCore.Instance.UnitTraceService.DeleteTask(_currentBdTask.TaskID,false))
                {
                    EventAggregator.PublishMessage(new GoTaskListViewEvent());
                }
                else
                {
                    var md = new MetroDialogSettings();
                    md.AffirmativeButtonText = "好的";
                    await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "错误",
                    UnitCore.Instance.UnitTraceService.Error, MessageDialogStyle.Affirmative, md);
                }
            }
        }
        public ICommand StopTaskCommand
        {
            get { return GetPropertyValue(() => StopTaskCommand); }
            set { SetPropertyValue(() => StopTaskCommand, value); }
        }


        private void CanExecuteStopTaskCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = false;
            if (TaskState == "WORKING")
                eventArgs.CanExecute = true;
        }


        private void ExecuteStopTaskCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if (_currentBdTask != null&&_currentBdTask.TaskStage==(int)UBP.TaskStage.Continue)
            {
                DeckDataProtocol.Stop();
                IUpdateTaskHandle(null, null);
            }
        }
        public ICommand StartTaskCommand
        {
            get { return GetPropertyValue(() => StartTaskCommand); }
            set { SetPropertyValue(() => StartTaskCommand, value); }
        }

        


        private void CanExecuteStartTaskCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = false;
            if (TaskState == "WORKING")
                eventArgs.CanExecute = true;
        }


        private async void ExecuteStartTaskCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if (_currentBdTask != null)
            {
                if(TaskState=="UNSTART")
                    if (DeckDataProtocol.StartTask(_currentBdTask.TaskID) == _currentBdTask.TaskID)//正确开始任务
                    {
                        
                        var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                        dialog.Title = "开始任务";
                        await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                            dialog);
                        var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
                        textBlock.Text = "发送成功！";
                        TaskState = "WORKING";
                        IsWorking = true;
                        await TaskEx.Delay(1000);
                        await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, dialog);
                    }
                    else
                    {
                        var md = new MetroDialogSettings();
                        md.AffirmativeButtonText = "好的";
                        await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "错误",
                        "任务开始失败", MessageDialogStyle.Affirmative, md);
                        TaskState = "UNSTART";
                        IsWorking = false;
                    }
                    IUpdateTaskHandle(null, null);
            }
        }
        //状态更新：超时回调加操作动作
        public async void IUpdateTaskHandle(object sender, EventArgs e)
        {
            _currentBdTask = DeckDataProtocol.WorkingBdTask;
            switch (_currentBdTask.TaskStage)
            {
                case -1:
                    TaskStatus = "任务失败，错误码="+DeckDataProtocol.ErrorCode.ToString();
                    IsWorking = false;
                    TaskState = "STOP";
                    var md = new MetroDialogSettings();
                    md.AffirmativeButtonText = "好的";
                    await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "提示",
                    "任务失败", MessageDialogStyle.Affirmative, md);
                    break;
                case 0:
                    TaskStatus = "任务开始";
                    TaskState = "UNSTART";
                    IsWorking = false;
                    break;
                case 1:
                    TaskStatus = "等待数据";
                    TaskState = "STOP";
                    IsWorking = false;
                    break;
                case 2:
                    TaskStatus = "正在唤醒设备";
                    TaskState = "STOP";
                    IsWorking = false;
                    break;
                case 3:
                    TaskStatus = "数据准备完毕";
                    TaskState = "WORKING";
                    IsWorking = true;
                    break;
                case 4:
                    TaskStatus = "数据传输中……";
                    TaskState = "WORKING";
                    IsWorking = true;
                    break;
                case 5:
                    TaskStatus = "暂停";
                    TaskState = "STOP";
                    IsWorking = false;
                    break;
                case 6:
                    TaskStatus = "任务完成";
                    TaskState = "COMPLETED";
                    IsWorking = false;
                    break;
                default:
                    TaskStatus = "未知错误";
                    TaskState = "UNSTART";
                    IsWorking = false;
                    break;
            }
            LastTime = _currentBdTask.LastTime;
            TotalSeconds = _currentBdTask.TotolTime;
            RecvBytes = _currentBdTask.RecvBytes;
            RetryRate = (double)_currentBdTask.ErrIdxStr.Split(';').Count() / 7;
        }
        
        
    }
}
