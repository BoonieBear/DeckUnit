using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using BoonieBear.DeckUnit.ACNP;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.Events;
using BoonieBear.DeckUnit.ICore;
using BoonieBear.DeckUnit.JsonUtils;
using BoonieBear.DeckUnit.Models;
using Microsoft.Win32;
using TinyMetroWpfLibrary.Controller;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System.IO;
using BoonieBear.DeckUnit.ViewModels;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.BaseType;
namespace BoonieBear.DeckUnit.Views
{

    public partial class MainFrame
    {
        private Stream Updatefile;
        private string DownLoadFile = "";
        private DispatcherTimer t;
        private DispatcherTimer networkTimer;
        public MainFrame()
        {
            InitializeComponent();
            MainFrameViewModel.pMainFrame.DialogCoordinator = DialogCoordinator.Instance;
            //DataContext = MainFrameViewModel.pMainFrame;
            
            Kernel.Instance.Controller.SetRootFrame(ContentFrame);
            //this.DebugLog.Text = MainFrameViewModel.pMainFrame.Shellstring;
            filterbox.Items.CurrentChanged += (_1, _2) => filterbox.ScrollIntoView(filterbox.Items[filterbox.Items.Count-1]);

        }

        private void ContentFrame_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

            ProgressDialogController remote = null;
            
            UnitCore.Instance.EventAggregator.PublishMessage(new GoHomePageNavigationEvent());
            UnitCore.Instance.Start();
            if(networkTimer==null)
                networkTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(2000), DispatcherPriority.Background, RefreshNetStatus, Dispatcher.CurrentDispatcher);
            networkTimer.Start();
        }

        private void RefreshNetStatus(object sender, EventArgs e)
        {
            if (UnitCore.GetInstance().NetEngine.IsWorking)
            {
                NetLinkCheckBox.IsChecked = true;
            }
            else
            {
                NetLinkCheckBox.IsChecked = false;
            }
        }

        private void FlyOutView(int index)
        {
            var flyout = this.flyoutsControl.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }

            flyout.IsOpen = !flyout.IsOpen;
        }
        
        private void LaunchDebugView(object sender, System.Windows.RoutedEventArgs e)
        {
            var flyout = this.flyoutsControl.Items[2] as Flyout;
            flyout.IsOpen = false;
            flyout = this.flyoutsControl.Items[1] as Flyout;
            flyout.IsOpen = false;
            FlyOutView(0);
        }

        
        private void LaunchDataView(object sender, System.Windows.RoutedEventArgs e)
        {
            var flyout = this.flyoutsControl.Items[2] as Flyout;
            flyout.IsOpen = false;
            flyout = this.flyoutsControl.Items[0] as Flyout;
            flyout.IsOpen = false;
            FlyOutView(1);
            MainFrameViewModel.pMainFrame.RecvMessage = 0;
        }

        private async void NetLinkCheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (NetLinkCheckBox.IsChecked == true && UnitCore.Instance.NetEngine.IsWorking==false)
            {
                UnitCore.Instance.NetEngine.Initialize();
                UnitCore.Instance.NetEngine.Start();
            }
            else
            {
                if (UnitCore.Instance.NetEngine.IsWorking == true)
                    UnitCore.Instance.NetEngine.Stop();
            }
        }

        private async void ToggleButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
             
            if (LoadcheckButton.IsChecked==true)
            {
                
                var LoaderCheck = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "确定",
                    NegativeButtonText = "取消",
                    //FirstAuxiliaryButtonText = "Cancel",
                    ColorScheme = MetroDialogOptions.ColorScheme
                };
                MessageDialogResult result = await this.ShowMessageAsync("串口", "进入Loader模式？",
                MessageDialogStyle.AffirmativeAndNegative, LoaderCheck);
                if (result == MessageDialogResult.Affirmative)
                {
                    var cmd = MSPHexBuilder.Pack242();
                    var ret = UnitCore.Instance.CommEngine.SendCMD(cmd);
                    await ret;
                    if(ret.Result)
                        UnitCore.Instance.CommEngine.SerialService.ChangeMode(SerialServiceMode.LoaderMode);
                }
                else
                {
                    LoadcheckButton.IsChecked = false;
                }
                return;
            }
            if (LoadcheckButton.IsChecked==false)
            {
                
                var LoaderCheck = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "确定",
                    NegativeButtonText = "取消",
                    //FirstAuxiliaryButtonText = "Cancel",
                    ColorScheme = MetroDialogOptions.ColorScheme
                };
                MessageDialogResult result = await this.ShowMessageAsync("串口", "退出Loader模式？",
                MessageDialogStyle.AffirmativeAndNegative, LoaderCheck);
                if (result == MessageDialogResult.Affirmative)
                {
                    UnitCore.Instance.CommEngine.SerialService.ChangeMode(SerialServiceMode.HexMode);
                }
                else
                {
                    LoadcheckButton.IsChecked = true;
                }
                return;
            }
            
        }

        private async void SelectFileButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog OpenMspFile =new OpenFileDialog();
            if (OpenMspFile.ShowDialog() == true)
            {
                try
                {
                    if (UnitCore.Instance.CommEngine.IsWorking)
                    {
                        await UnitCore.Instance.CommEngine.SendFile(OpenMspFile.OpenFile());
                        UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("下载数据完成", LogType.OnlyInfo));
                    }
                    else
                    {
                        UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("串口进程未正常工作", LogType.OnlyInfo));
                    }
                    
                }
                catch (Exception MyEx)
                {

                    UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(MyEx, LogType.Both));
                }
            }
        }

        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
             var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "确定",
                NegativeButtonText = "取消",
                AnimateShow = true,
                AnimateHide = false
            };

            var result = await this.ShowMessageAsync("退出",
                "确信要退出程序吗？",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            

            if (result == MessageDialogResult.Affirmative)
            {
                UnitCore.Instance.Stop();
                System.Windows.Application.Current.Shutdown();
            }
                
        }

        private void FilterableListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            try
            {
                CommandLog cl = (CommandLog)DataListView.SelectedItem;
                if (cl == null)
                    return;
                if (cl.FilePath == null)
                    return;
                var fr = File.Open(cl.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var br = new BinaryReader(fr);
                //UnitCore.Instance.AcnMutex.WaitOne();
                ACNProtocol.GetDataForParse(br.ReadBytes((int)fr.Length));
                if (ACNProtocol.Parse())
                {
                    var tree = StringListToTree.TransListToNodeWriteLineic(ACNProtocol.parselist);
                    //UnitCore.Instance.AcnMutex.ReleaseMutex();
                    var datatree = new DataTreeModel(tree);
                    this._tree.Model = datatree;
                    MainFrameViewModel.pMainFrame.DataRecvTime = cl.LogTime.ToString();
                    var flyout = this.flyoutsControl.Items[2] as Flyout;
                    flyout.IsOpen = true;
                }
                else
                {
                    //UnitCore.Instance.AcnMutex.ReleaseMutex();
                    UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(new Exception(ACNProtocol.Errormessage), LogType.Both));
                }
            }
            catch (Exception ex)
            {
                //UnitCore.Instance.AcnMutex.ReleaseMutex();
                UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(ex, LogType.Both));
            }
            

        }

        private async void SetADBtn(object sender, System.Windows.RoutedEventArgs e)
        {
            var newdialog = (BaseMetroDialog) App.Current.MainWindow.Resources["SetADDialog"];
            var NumericUpDown = newdialog.FindChild<NumericUpDown>("ADNumUp");
            var cmd = MSPHexBuilder.Pack255((int)NumericUpDown.Value);
            var result = UnitCore.Instance.CommEngine.SendCMD(cmd);
            await result;
            var ret = result.Result;
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "确定";
            if(ret==false)
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "发送失败",
                UnitCore.Instance.NetEngine.Error,MessageDialogStyle.Affirmative,md);
            else
            {
                var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                dialog.Title = "设备命令";
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                    dialog);

                var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
                textBlock.Text = "发送成功！";

                await TaskEx.Delay(2000);

                await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame,dialog);
                await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, newdialog);
            }
        }

        private async void CloseSetAD(object sender, System.Windows.RoutedEventArgs e)
        {
            await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, (BaseMetroDialog)App.Current.MainWindow.Resources["SetADDialog"]);
        }

        private async void SetWakeBtn(object sender, System.Windows.RoutedEventArgs e)
        {
            var newdialog = (BaseMetroDialog)App.Current.MainWindow.Resources["SetWakeUpDialog"];
            var NumericUpDown1 = newdialog.FindChild<NumericUpDown>("Com2WakeNumUp");
            var NumericUpDown2 = newdialog.FindChild<NumericUpDown>("Com3WakeNumUp");
            var cmd = MSPHexBuilder.Pack248((int)NumericUpDown1.Value, (int)NumericUpDown2.Value);
            var result = UnitCore.Instance.CommEngine.SendCMD(cmd);
            await result;
            var ret = result.Result;
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "确定";
            if (ret == false)
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "发送失败",
                UnitCore.Instance.NetEngine.Error,MessageDialogStyle.Affirmative,md);
            else
            {
                var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                dialog.Title = "设备命令";
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                    dialog);

                var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
                textBlock.Text = "发送成功！";

                await TaskEx.Delay(2000);

                await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, dialog);
                await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, newdialog);
            }
        }

        private async void CloseWakeTimeBtn(object sender, System.Windows.RoutedEventArgs e)
        {
            await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, (BaseMetroDialog)App.Current.MainWindow.Resources["SetWakeUpDialog"]);
        }

        private void DebugLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            DebugLog.ScrollToEnd();
        }

       

        private void DataListView_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                CommandLog cl = (CommandLog)DataListView.SelectedItem;
                if (cl == null)
                    return;
                var fr = File.Open(cl.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var br = new BinaryReader(fr);
                UnitCore.Instance.AcnMutex.WaitOne();
                ACNProtocol.GetDataForParse(br.ReadBytes((int)fr.Length));
                if (ACNProtocol.Parse())
                {
                    var tree = StringListToTree.TransListToNodeWriteLineic(ACNProtocol.parselist);
                    UnitCore.Instance.AcnMutex.ReleaseMutex();
                    var datatree = new DataTreeModel(tree);
                    this._tree.Model = datatree;
                    MainFrameViewModel.pMainFrame.DataRecvTime = cl.LogTime.ToString();
                    var flyout = this.flyoutsControl.Items[2] as Flyout;
                    flyout.IsOpen = true;
                }
                else
                {
                    UnitCore.Instance.AcnMutex.ReleaseMutex();
                    UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(new Exception(ACNProtocol.Errormessage), LogType.Both));
                }
            }
            catch (Exception ex)
            {
                UnitCore.Instance.AcnMutex.ReleaseMutex();
                UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(ex, LogType.Both));
            }
        }

        private void DataListView_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
  
        }

        private async void SetSupplyBtn(object sender, System.Windows.RoutedEventArgs e)
        {
            var newdialog = (BaseMetroDialog)App.Current.MainWindow.Resources["SupplySetDialog"];
            var HighSetBox = newdialog.FindChild<ComboBox>("HighSet");
            var LowSetBox = newdialog.FindChild<ComboBox>("LowSet");
            var cmd = MSPHexBuilder.Pack241(HighSetBox.SelectedIndex, LowSetBox.SelectedIndex);
            var result = UnitCore.Instance.CommEngine.SendCMD(cmd);
            await result;
            var ret = result.Result;
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "确定";
            if (ret == false)
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "发送失败",
                UnitCore.Instance.NetEngine.Error, MessageDialogStyle.Affirmative, md);
            else
            {
                var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                dialog.Title = "外电配置命令";
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                    dialog);

                var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
                textBlock.Text = "发送成功！";

                await TaskEx.Delay(2000);

                await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, dialog);
                await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, newdialog);
            }
        }

        private async void CloseSupplyBtn(object sender, System.Windows.RoutedEventArgs e)
        {
            await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, (BaseMetroDialog)App.Current.MainWindow.Resources["SupplySetDialog"]);
        }
        private async void LaunchDownLoadView(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["DownloadDialog"];
            await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                dialog);
        }

        private async void SendFileBtn(object sender, System.Windows.RoutedEventArgs e)
        {
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "确定";
            if (DownLoadFile == "" || Updatefile==null)
            {    await
                    MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame,
                        "错误",
                       "请选择一个下载文件", MessageDialogStyle.Affirmative, md);
                return;
            }

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "确定",
                NegativeButtonText = "取消",
                AnimateShow = true,
                AnimateHide = false
            };

            var result = await this.ShowMessageAsync("下载",
                "确定要下载文件?-" + DownLoadFile,
                MessageDialogStyle.AffirmativeAndNegative, mySettings);
            if (result != MessageDialogResult.Affirmative)
                return;
            
            try
            {
                if (UnitCore.Instance.NetEngine.IsWorking)
                {
                    t = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Background, RefreshPercentage, Dispatcher.CurrentDispatcher);
                    t.Start();
                    var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["DownloadDialog"];
                    var statusbar = dialog.FindChild<MetroProgressBar>("Percentbar");
                    statusbar.Visibility = Visibility.Visible;
                    statusbar.Value = 0;
                    ComboBox box = dialog.FindChild<ComboBox>("SelectModeBox");
                    DownLoadFileType dt = DownLoadFileType.Wave;
                    switch (box.SelectedIndex)
                    {
                        case 0:
                            dt = DownLoadFileType.Wave;
                            break;
                        case 1:
                            dt = DownLoadFileType.RltUpdate;
                            break;
                        case 2:
                            dt = DownLoadFileType.FixFirm;
                            break;
                        case 3:
                            dt = DownLoadFileType.FloatM2;
                            break;
                        case 4:
                            dt = DownLoadFileType.FloatM4;
                            break;
                        case 5:
                            dt = DownLoadFileType.FPGA;
                            break;
                        case 6:
                            dt = DownLoadFileType.BootLoader;
                            break;
                        default:
                            dt = DownLoadFileType.Wave;
                            break;
                    }
                    await UnitCore.Instance.NetEngine.DownloadFile(Updatefile,dt);
                    UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("下载完成:" + DownLoadFile, LogType.OnlyInfo));
                    Updatefile.Close();
                    statusbar.Visibility = Visibility.Collapsed;
                    if (t != null)
                        t.Stop();
                }
                else
                {
                    UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("当前没有网络连接", LogType.OnlyInfo));
                }

            }
            catch (Exception MyEx)
            {
                Updatefile.Close();
                UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(MyEx, LogType.Both));
            }
        }

        private void RefreshPercentage(object sender, EventArgs e)
        {
            var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["DownloadDialog"];
            var statusbar = dialog.FindChild<MetroProgressBar>("Percentbar");
            statusbar.Value = UnitCore.Instance.NetEngine.SendBytes * 100 / (int)Updatefile.Length;
        }

        private async void CloseDownloadDialog(object sender, System.Windows.RoutedEventArgs e)
        {
            if (t != null)
                t.Stop();
            var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["DownloadDialog"];
            var statusbar = dialog.FindChild<MetroProgressBar>("Percentbar");
            statusbar.Value = 0;
            statusbar.Visibility = Visibility.Collapsed;
            var status = dialog.FindChild<TextBlock>("StatusBlock");
            status.Text = "";
            DownLoadFile = "";
            if (Updatefile!=null)
                Updatefile.Close();
            await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, (BaseMetroDialog)App.Current.MainWindow.Resources["DownloadDialog"]);
        }

        private async void SelectDownLoadFile(object sender, System.Windows.RoutedEventArgs e)
        {
            var newdialog = (BaseMetroDialog)App.Current.MainWindow.Resources["DownloadDialog"];
            var status = newdialog.FindChild<TextBlock>("StatusBlock");
            var btn = newdialog.FindChild<Button>("DownLoadBtn");
            var box = newdialog.FindChild<ComboBox>("SelectModeBox");
            OpenFileDialog OpenFileDlg = new OpenFileDialog();
            if (OpenFileDlg.ShowDialog() == true)
            {
                Updatefile = OpenFileDlg.OpenFile();
                DownLoadFile = OpenFileDlg.FileName;
                status.Text =OpenFileDlg.SafeFileName;
                btn.IsEnabled = true;
            }
        }
    }
}
