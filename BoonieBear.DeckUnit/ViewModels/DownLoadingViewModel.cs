using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Threading;
using BoonieBear.DeckUnit.ACNP;
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
using System.ComponentModel;
namespace BoonieBear.DeckUnit.ViewModels
{
    public class DownLoadingViewModel : ViewModelBase, BoonieBear.DeckUnit.UBP.BDObserver<EventArgs>
    {
        private BDTask _currentBdTask;
        private DispatcherTimer t;
        public override void Initialize()
        {
            CmdId = 1;
            
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            GoDataCommand = RegisterCommand(ExecuteGoDataCommand, CanExecuteGoDataCommand, true);
            StartTaskCommand = RegisterCommand(ExecuteStartTaskCommand, CanExecuteStartTaskCommand, true);
            StopTaskCommand = RegisterCommand(ExecuteStopTaskCommand, CanExecuteStopTaskCommand, true);
            DeleteTaskCommand = RegisterCommand(ExecuteDeleteTaskCommand, CanExecuteDeleteTaskCommand, true);
            RetryCommand = RegisterCommand(ExecuteRetryCommand, CanExecuteRetryCommand, true);
            IsWorking = false;
            TaskState = "UNSTART";
            IUpdateTaskHandle(null,null);
        }


        public override void InitializePage(object extraData)
        {
            var message = extraData as GoDownLoadingViewEvent;
            if (message != null)
            {
                _currentBdTask = message.NewBdTask;
                if (_currentBdTask == null)
                    return;
                TaskID = _currentBdTask.TaskID;
                DestID = _currentBdTask.DestID;
                CmdId = _currentBdTask.CommID;
                //StarTime = _currentBdTask.StarTime;
                TotalSeconds = 0;
                DeckDataProtocol.AddCallBack(this);
                IUpdateTaskHandle(null,null);
            }

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
            get { return GetPropertyValue(() => CmdId); }
            set
            {
                SetPropertyValue(() => CmdId, value);
                switch (CmdId)
                {
                    case 1:
                        CmdIDDesc = "@SP";
                        break;
                    case 2:
                        CmdIDDesc = "时间段\n " + _currentBdTask.ParaBytes[0].ToString().PadLeft(2, '0') +
                                    _currentBdTask.ParaBytes[1].ToString().PadLeft(2, '0') +
                                    _currentBdTask.ParaBytes[2].ToString().PadLeft(2, '0') +
                                    _currentBdTask.ParaBytes[3].ToString().PadLeft(2, '0') +
                                    _currentBdTask.ParaBytes[4].ToString().PadLeft(2, '0') + "-" +
                                    _currentBdTask.ParaBytes[5].ToString().PadLeft(2, '0') +
                                    _currentBdTask.ParaBytes[6].ToString().PadLeft(2, '0') +
                                    _currentBdTask.ParaBytes[7].ToString().PadLeft(2, '0') +
                                    _currentBdTask.ParaBytes[8].ToString().PadLeft(2, '0') +
                                    _currentBdTask.ParaBytes[9].ToString().PadLeft(2, '0');

                        break;
                    case 3:
                        CmdIDDesc = "抽取索取-" + Encoding.Default.GetString(_currentBdTask.ParaBytes);
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
        public  string LeftTime
        {
            get { return GetPropertyValue(() => LeftTime); }
            set { SetPropertyValue(() => LeftTime, value); }
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

            if (TaskState == "WORKING" || TaskState == "WAITING") //正在工作
                {
                    var md = new MetroDialogSettings();
                    md.AffirmativeButtonText = "确定";
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
        }


        private  void ExecuteGoDataCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            //EventAggregator.PublishMessage(new GoADCPDataViewEvent(_currentBdTask));
            Process ps = new Process();
            ps.StartInfo.FileName = "explorer.exe";
            ps.StartInfo.Arguments =_currentBdTask.FilePath;
            ps.Start();
        }
        public ICommand RetryCommand
        {
            get { return GetPropertyValue(() => RetryCommand); }
            set { SetPropertyValue(() => RetryCommand, value); }
        }


        private void CanExecuteRetryCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private async void ExecuteRetryCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            try
            {
                ACNBuilder.PackTask(DeckDataProtocol.WorkingBdTask, false, DeckDataProtocol.LastRecvPkgId);
            }
            catch (Exception exception)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(exception, LogType.Both));
                return;
            }
            var cmd = ACNProtocol.Package(false);
            var result = UnitCore.Instance.NetEngine.SendCMD(cmd);
            await result;
            var end = result.Result;
            if (end == false)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(
                    new LogEvent(UnitCore.Instance.NetEngine.Error, LogType.Both));
            }
        }
        public ICommand DeleteTaskCommand
        {
            get { return GetPropertyValue(() => DeleteTaskCommand); }
            set { SetPropertyValue(() => DeleteTaskCommand, value); }
        }


        private void CanExecuteDeleteTaskCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
                eventArgs.CanExecute = true;
        }


        private async void ExecuteDeleteTaskCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if (_currentBdTask != null)
            {
                DeckDataProtocol.Stop();
                t.Stop();
                if (UnitCore.Instance.UnitTraceService.DeleteTask(_currentBdTask.TaskID,false))
                {
                    
                    EventAggregator.PublishMessage(new GoBackNavigationRequest());
                }
                else
                {
                    var md = new MetroDialogSettings();
                    md.AffirmativeButtonText = "确定";
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
            eventArgs.CanExecute = true;
        }


        private async void ExecuteStopTaskCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if (_currentBdTask != null)
            {
                var md = new MetroDialogSettings();
                md.AffirmativeButtonText = "确定";
                md.NegativeButtonText = "取消";
                var ret =  MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "提示",
                "确定要取消任务吗？", MessageDialogStyle.AffirmativeAndNegative, md);
                await ret;
                if (ret.Result == MessageDialogResult.Affirmative)
                {
                    DeckDataProtocol.Stop();
                    t.Stop();
                    IUpdateTaskHandle(null, null);
                    UnitCore.Instance.EventAggregator.PublishMessage(new GoBackNavigationRequest());
                }
            }
        }
        public ICommand StartTaskCommand
        {
            get { return GetPropertyValue(() => StartTaskCommand); }
            set { SetPropertyValue(() => StartTaskCommand, value); }
        }

        


        private void CanExecuteStartTaskCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
                eventArgs.CanExecute = true;
        }


        private async void ExecuteStartTaskCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if (_currentBdTask != null)
            {
                if(TaskState=="UNSTART"||TaskState=="STOP")
                    if (DeckDataProtocol.StartTask(_currentBdTask.TaskID) == _currentBdTask.TaskID) //正确开始任务
                    {
                        ACNBuilder.PackTask(_currentBdTask, true, -1);
                        var cmd = ACNProtocol.Package(false);
                        var result = UnitCore.Instance.NetEngine.SendCMD(cmd);
                        await result;
                        var end = result.Result;
                        if (end == true)
                        {

                            var dialog = (BaseMetroDialog) App.Current.MainWindow.Resources["CustomInfoDialog"];
                            dialog.Title = "开始任务";
                            await
                                MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(
                                    MainFrameViewModel.pMainFrame,
                                    dialog);
                            var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
                            textBlock.Text = "发送成功！";
                            TaskState = "WORKING";
                            IsWorking = true;
                            TotalSeconds = 0;
                            if(t!=null)
                                t.Stop();
                            t = new DispatcherTimer(TimeSpan.FromSeconds(1),DispatcherPriority.Background, Time_Tick,Dispatcher.CurrentDispatcher);
                            await TaskEx.Delay(1000);
                            await
                                MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(
                                    MainFrameViewModel.pMainFrame, dialog);
                        }
                        else
                        {
                            UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent(UnitCore.Instance.NetEngine.Error, LogType.Both));
                            DeckDataProtocol.Stop();
                        }
                    }
                    else
                    {
                        var md = new MetroDialogSettings();
                        md.AffirmativeButtonText = "确定";
                        await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "错误",
                        "任务开始失败", MessageDialogStyle.Affirmative, md);
                        TaskState = "UNSTART";
                        IsWorking = false;
                    }
                    IUpdateTaskHandle(null, null);
            }
        }

        private void Time_Tick(object sender, EventArgs e)
        {
            TotalSeconds++;
        }
        //状态更新：超时回调加操作动作
        public async void IUpdateTaskHandle(object sender, EventArgs e)
        {
            Dispatcher.CurrentDispatcher.Invoke(new Action(async () =>  
            {
                _currentBdTask = DeckDataProtocol.WorkingBdTask;
                switch (_currentBdTask.TaskStage)
                {
                    case (int) TaskStage.Failed:
                        TaskStatus = "任务失败，错误码=" + DeckDataProtocol.ErrorCode.ToString();
                        IsWorking = false;
                        TaskState = "STOP";
                        var md = new MetroDialogSettings();
                        md.AffirmativeButtonText = "确定";
                        App.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(
                                MainFrameViewModel.pMainFrame, "提示",
                                "任务失败", MessageDialogStyle.Affirmative, md);
                        }));
                        t.Stop();
                        break;
                    case (int) TaskStage.UnStart:
                        TaskStatus = "任务未开始";
                        TaskState = "UNSTART";
                        IsWorking = false;
                        break;
                    case (int) TaskStage.WaitForAns:
                        TaskStatus = "等待应答";
                        TaskState = "WAITING";
                        IsWorking = true;
                        break;
                    case (int) TaskStage.Waiting:
                        TaskStatus = "准备数据……";
                        TaskState = "STOP";
                        IsWorking = true;
                        break;
                    case (int) TaskStage.Waking:
                        TaskStatus = "正在唤醒设备";
                        TaskState = "STOP";
                        IsWorking = true;
                        break;
                    case (int) TaskStage.OK:
                        TaskStatus = "数据准备完毕";
                        TaskState = "WORKING";
                        IsWorking = true;
                        break;
                    case (int) TaskStage.Continue:
                        TaskStatus = "数据传输中……";
                        TaskState = "WORKING";
                        IsWorking = true;
                        break;
                    case (int) TaskStage.Pause:
                        TaskStatus = "暂停";
                        TaskState = "STOP";
                        IsWorking = false;
                        break;
                    case (int) TaskStage.Finish:
                        TaskStatus = "任务完成";
                        TaskState = "COMPLETED";
                        IsWorking = false;
                        t.Stop();
                        break;
                    case (int) TaskStage.RecvReply:
                        TaskStatus = "发送下一组任务数据";
                        TaskState = "WAITING";
                        IsWorking = true;
                        try
                        {
                            ACNBuilder.PackTask(DeckDataProtocol.WorkingBdTask, false, DeckDataProtocol.LastRecvPkgId);
                        }
                        catch (Exception exception)
                        {
                            UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(exception, LogType.Both));
                            break;
                        }
                        var cmd = ACNProtocol.Package(false);
                        var result = UnitCore.Instance.NetEngine.SendCMD(cmd);
                        await result;
                        var end = result.Result;
                        if (end == false)
                        {
                            UnitCore.Instance.EventAggregator.PublishMessage(
                                new LogEvent(UnitCore.Instance.NetEngine.Error, LogType.Both));
                        }
                        _currentBdTask.TaskStage = (int) TaskStage.WaitForAns;
                        break;
                    default:
                        TaskStatus = "未知错误";
                        TaskState = "UNSTART";
                        IsWorking = false;
                        break;
                }
                LastTime = _currentBdTask.LastTime;
                //TotalSeconds = _currentBdTask.TotolTime;
                if (_currentBdTask.TotalPkg > 0)
                    LeftTime =
                        TimeSpan.FromSeconds((_currentBdTask.TotalPkg - DeckDataProtocol.RecvPkg) * DeckDataProtocol.SendRecvPeriod+15).ToString("g");
                else
                    LeftTime = "";
                RecvBytes = _currentBdTask.RecvBytes;
                if (_currentBdTask.ErrIdxStr != "")
                    RetryRate = (double) _currentBdTask.ErrIdxStr.Split(';').Count()/15;
                else
                    RetryRate = 0;
            }));
        }

    }
}
