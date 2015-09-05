using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.Helps;
using BoonieBear.DeckUnit.UBP;
using MahApps.Metro.Controls.Dialogs;
using TinyMetroWpfLibrary.EventAggregation;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;
using BoonieBear.DeckUnit.Events;
using Task = BoonieBear.DeckUnit.DAL.Task;

namespace BoonieBear.DeckUnit.ViewModels
{
    public class DownLoadingViewModel:ViewModelBase,IHandleMessage<UpdateCurrentTask>
    {
        private Task currentTask;
        public override void Initialize()
        {
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            GoDataCommand = RegisterCommand(ExecuteGoDataCommand, CanExecuteGoDataCommand, true);
            StartTaskCommand = RegisterCommand(ExecuteStartTaskCommand, CanExecuteStartTaskCommand, true);
            DeleteTaskCommand = RegisterCommand(ExecuteDeleteTaskCommand, CanExecuteDeleteTaskCommand, true);
            IsWorking = false;
            TaskState = "UNSTART";
        }


        public override void InitializePage(object extraData)
        {
            currentTask = extraData as Task;
            if (currentTask == null)
                return;
            TaskID = currentTask.TaskID;
            DestID = currentTask.DestID;
            CmdID = currentTask.CommID;
            StarTime = currentTask.StarTime;
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
        public  int CmdID
        {
            get { return GetPropertyValue(() => CmdID); }
            set { SetPropertyValue(() => CmdID, value); }
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
            if(currentTask!=null)
                if(currentTask.TaskState!=(int) TaskStage.Finish)
                    eventArgs.CanExecute = false;
        }


        private  void ExecuteGoDataCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoADCPDataViewEvent(currentTask));
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
            if (currentTask != null)
            {
                if (UnitCore.Instance.UnitTraceService.DeleteTask(currentTask.TaskID,false))
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
            if (currentTask != null&&currentTask.TaskState==(int)UBP.TaskStage.Continue)
            {
                DeckDataProtocol.Stop();
                EventAggregator.PublishMessage(new UpdateCurrentTask());
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
            if (currentTask != null)
            {
                if(TaskState=="UNSTART")
                    if (DeckDataProtocol.ContinueTask(currentTask.TaskID) == currentTask.TaskID)
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
                        UnitCore.Instance.NetEngine.Error, MessageDialogStyle.Affirmative, md);
                        TaskState = "UNSTART";
                        IsWorking = false;
                    }
                EventAggregator.PublishMessage(new UpdateCurrentTask());
            }
        }
        /// <summary>
        /// 收到数据后观察者通知currenttask更新
        /// </summary>
        /// <param name="message"></param>
        public async void Handle(UpdateCurrentTask message)
        {
            currentTask = DeckDataProtocol.WorkingTask;
            switch (currentTask.TaskState)
            {
                case -1:
                    TaskStatus = "任务失败";
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
                    TaskState = "WORKING";
                    IsWorking = true;
                    break;
                case 2:
                    TaskStatus = "传输数据中";
                    TaskState = "WORKING";
                    IsWorking = true;
                    break;
                case 3:
                    TaskStatus = "暂停";
                    TaskState = "STOP";
                    IsWorking = false;
                    break;
                case 4:
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
            LastTime = currentTask.LastTime;
            TotalSeconds = currentTask.TotolTime;
            RecvBytes = currentTask.RecvBytes;
            var rtor = currentTask.ErrIndex.GetEnumerator();
            var errindex = 0;
            while (rtor.MoveNext())
            {
                if ((bool) rtor.Current)
                    errindex++;
            }
            RetryRate = (double)errindex/8;
        }
    }
}
