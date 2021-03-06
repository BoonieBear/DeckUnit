﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BoonieBear.DeckUnit.ACNP;
using BoonieBear.DeckUnit.BaseType;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.Events;
using BoonieBear.DeckUnit.Helps;
using BoonieBear.DeckUnit.UBP;
using MahApps.Metro.Controls.Dialogs;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;

namespace BoonieBear.DeckUnit.ViewModels
{
    public class NewTaskPageViewModel : ViewModelBase
    {
        public override void Initialize()
        {
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            CreateTask = RegisterCommand(ExecuteCreateTask, CanExecuteCreateTask, true);
            CommIndex = 1;
            DestID = 1;
            TypeIndex = 0;
            SelectedFromDate = DateTime.Now;
            SelectedToDate = DateTime.Now;
            SelectedFetchDate = DateTime.Now;
            SelectedFromTime = DateTime.Now;
            SelectedToTime = DateTime.Now;
            SelectedFetchTime = DateTime.Now;
        }


        public override void InitializePage(object extraData)
        {
        }


        #region 绑定属性
        public int DestID
        {
            get { return GetPropertyValue(() => DestID); }
            set { SetPropertyValue(() => DestID, value); }
        }
        public int CommIndex
        {
            get { return GetPropertyValue(() => CommIndex); }
            set { SetPropertyValue(() => CommIndex, value); }
        }
        public int TypeIndex
        {
            get { return GetPropertyValue(() => TypeIndex); }
            set
            {
                SetPropertyValue(() => TypeIndex, value);
                ShowCondition4 = false;
                ShowCondition1 = false;
                ShowCondition2 = false;
                ShowCondition3 = false;
                ShowCondition5 = false;
                switch (TypeIndex)
                {
                    case 0:
                        ShowCondition1 = true;
                        break;
                    case 1:
                        ShowCondition2 = true;
                        break;
                    case 2:
                        ShowCondition3 = true;
                        break;
                    case 3:
                        ShowCondition4 = true;
                        break;
                    case 4:
                        ShowCondition5 = true;
                        ShowCondition2 = true;
                        break;
                    default:
                        break;
                }
            }
        }

        public bool ShowCondition1
        {
            get { return GetPropertyValue(() => ShowCondition1); }
            set { SetPropertyValue(() => ShowCondition1, value); }
        }

        public bool ShowCondition2
        {
            get { return GetPropertyValue(() => ShowCondition2); }
            set { SetPropertyValue(() => ShowCondition2, value); }
        }
        public bool ShowCondition3
        {
            get { return GetPropertyValue(() => ShowCondition3); }
            set { SetPropertyValue(() => ShowCondition3, value); }
        }
        public bool ShowCondition4
        {
            get { return GetPropertyValue(() => ShowCondition4); }
            set { SetPropertyValue(() => ShowCondition4, value); }
        }
        public bool ShowCondition5
        {
            get { return GetPropertyValue(() => ShowCondition4); }
            set { SetPropertyValue(() => ShowCondition4, value); }
        }
        public int SamplingInterval
        {
            get { return GetPropertyValue(() => SamplingInterval); }
            set { SetPropertyValue(() => SamplingInterval, value); }
        }
        public int SamplingNum
        {
            get { return GetPropertyValue(() => SamplingNum); }
            set { SetPropertyValue(() => SamplingNum, value); }
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
        public DateTime SelectedFromTime
        {
            get { return GetPropertyValue(() => SelectedFromTime); }
            set { SetPropertyValue(() => SelectedFromTime, value); }
        }
        public DateTime SelectedToTime
        {
            get { return GetPropertyValue(() => SelectedToTime); }
            set { SetPropertyValue(() => SelectedToTime, value); }
        }
        public DateTime SelectedFetchDate
        {
            get { return GetPropertyValue(() => SelectedFetchDate); }
            set { SetPropertyValue(() => SelectedFetchDate, value); }
        }
        public DateTime SelectedFetchTime
        {
            get { return GetPropertyValue(() => SelectedFetchDate); }
            set { SetPropertyValue(() => SelectedFetchDate, value); }
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


        private void ExecuteGoBackCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }

        public ICommand CreateTask
        {
            get { return GetPropertyValue(() => CreateTask); }
            set { SetPropertyValue(() => CreateTask, value); }
        }


        private void CanExecuteCreateTask(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private async void ExecuteCreateTask(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if (!UnitCore.Instance.NetEngine.IsWorking)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("没有网络连接", LogType.OnlyInfo));
                return;
            }
            if (TypeIndex == 0 || TypeIndex == 2)
            {
                var md = new MetroDialogSettings();
                md.AffirmativeButtonText = "确定";
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "提示",
                "当前命令不支持", MessageDialogStyle.Affirmative, md);
                return;
            }
            var tasktype = (TaskType)Enum.Parse(typeof(TaskType),(TypeIndex + 1).ToString());
            byte[] paraBytes = null;
            if (TypeIndex == 0 || TypeIndex == 3)
                paraBytes = null;
            if (TypeIndex == 1 || TypeIndex == 4)
            {
                paraBytes= new byte[10];
                paraBytes[0] = (byte)(SelectedFromDate.Year-2000);
                paraBytes[1] = (byte)SelectedFromDate.Month;
                paraBytes[2] = (byte)SelectedFromDate.Day;
                paraBytes[3] = (byte)SelectedFromTime.Hour;
                paraBytes[4] = (byte)SelectedFromTime.Minute;
                paraBytes[5] = (byte)(SelectedToDate.Year - 2000);
                paraBytes[6] = (byte)SelectedToDate.Month;
                paraBytes[7] = (byte)SelectedToDate.Day;
                paraBytes[8] = (byte)SelectedToTime.Hour;
                paraBytes[9] = (byte)SelectedToTime.Minute;
            }
            if (TypeIndex == 2 )
            {
                paraBytes = new byte[6];
                paraBytes[0] = (byte)SelectedFetchDate.Month;
                paraBytes[1] = (byte)SelectedFetchDate.Day;
                paraBytes[2] = (byte)SelectedFetchTime.Hour;
                paraBytes[3] = (byte)SelectedFetchTime.Minute;
                paraBytes[4] = (byte) SamplingInterval;
                paraBytes[5] = (byte) SamplingNum;
            }
            var id = DeckDataProtocol.CreateNewTask(DestID, CommIndex + 2, tasktype, paraBytes);
            if (id <= 0)
            {
                var md = new MetroDialogSettings();
                md.AffirmativeButtonText = "确定";
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "提示",
                "创建新任务失败", MessageDialogStyle.Affirmative, md);
                return;
            }
            if (TypeIndex == 3 || TypeIndex == 4)
            {
                ACNBuilder.PackTask(DeckDataProtocol.WorkingBdTask, true, -1);
                var cmd = ACNProtocol.Package(false);
                
                var result = UnitCore.Instance.NetEngine.SendCMD(cmd);
                await result;
                var ret = result.Result;
                if (ret == false)
                {
                    UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("命令发送失败", LogType.Both));
                }
                else
                {
                    UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("命令发送成功", LogType.Both));
                }
                if (TypeIndex == 4)
                {
                    var md = new MetroDialogSettings();
                    md.AffirmativeButtonText = "确定";
                    await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame,
                        "正在查询数据", "查询数据需要一定时间，请稍后……", MessageDialogStyle.Affirmative, md);
                }
                return;
            }
            
            var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
            dialog.Title = "创建新任务";
            await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                dialog);
            var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
            textBlock.Text = "创建成功！";
            await TaskEx.Delay(1000);
            await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, dialog);
            EventAggregator.PublishMessage(new GoDownLoadingViewEvent(DeckDataProtocol.WorkingBdTask));
        }
    }
}
