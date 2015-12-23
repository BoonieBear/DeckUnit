using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BoonieBear.DeckUnit.ACMP;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using DevExpress.Xpf.Grid;
using ImageProc;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using TinyMetroWpfLibrary.Controller;
using MahApps.Metro.Controls.Dialogs;
using BoonieBear.DeckUnit.Mov4500UI.Core;
using BoonieBear.DeckUnit.Mov4500UI.ViewModel;
using TreeHelper = BoonieBear.DeckUnit.Mov4500UI.Helpers.TreeHelper;
using System.IO;

namespace BoonieBear.DeckUnit.Mov4500UI.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainFrame
    {
        private string selectimgpath;
        public MainFrame()
        {
            InitializeComponent();
            MainFrameViewModel.pMainFrame.DialogCoordinator = DialogCoordinator.Instance;
            Kernel.Instance.Controller.SetRootFrame(ContentFrame);
        }

        private  void ContentFrame_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            
            Application.Current.MainWindow = this;
            TaskEx.Run(()=>UnitCore.Instance.Start());
            
            Kernel.Instance.Controller.NavigateToPage("Views/HomePageView.xaml");
            BoonieBear.DeckUnit.Mov4500UI.Helpers.LogHelper.WriteLog("开始工作");
            Splasher.CloseSplash();
            
        }

        private void MetroWindow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private async void SendFH(object sender, RoutedEventArgs e)
        {
            var newdialog = (BaseMetroDialog)App.Current.MainWindow.Resources["SendFhDialog"];
            var textBox = newdialog.FindChild<TextBox>("FHBlock");
           
            if (textBox.Text.Length> 0)
            {
                UnitCore.Instance.AddFHHandle(textBox.Text);
                bool ret = UnitCore.Instance.NetCore.Send((int)ModuleType.FH, Encoding.Default.GetBytes(textBox.Text));
                await
                    MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame,
                        newdialog);
                var md = new MetroDialogSettings();
                md.AffirmativeButtonText = "确定";
                if (ret == false)
                    await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "发送失败",
                    UnitCore.Instance.NetCore.Error, MessageDialogStyle.Affirmative, md);
            }
            
        }

        private async void CloseFh(object sender, RoutedEventArgs e)
        {
            await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, (BaseMetroDialog)App.Current.MainWindow.Resources["SendFhDialog"]);

        }

        private void SelectImg(object sender, RoutedEventArgs e)
        {
            var newdialog = (BaseMetroDialog)App.Current.MainWindow.Resources["SendImgDialog"];
            var imgBox = newdialog.FindChild<System.Windows.Controls.Image>("SelectImageContainer");
            imgBox.Source = null;
            OpenFileDialog OpenImgFile = new OpenFileDialog();
            OpenImgFile.Filter="图片文件(*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp";
            if (OpenImgFile.ShowDialog() == true)
            {
                try
                {
                    selectimgpath = OpenImgFile.FileName;
                    imgBox.Source = new BitmapImage(new Uri(selectimgpath, UriKind.Absolute));

                }
                catch (Exception MyEx)
                {
                    UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(MyEx, LogType.Both));
                }
            }
        }

        private async void SendImg(object sender, RoutedEventArgs e)
        {
            var newdialog = (BaseMetroDialog)App.Current.MainWindow.Resources["SendImgDialog"];
            var imgBox = newdialog.FindChild<System.Windows.Controls.Image>("SelectImageContainer");
            if(imgBox.Source==null)
                return;
            var img = new System.Windows.Controls.Image();
            img.Source = imgBox.Source;
            UnitCore.Instance.AddImgHandle(img);
            if (Jp2KConverter.LoadImage(selectimgpath))
            {
                var buf = Jp2KConverter.SaveJp2K(UnitCore.Instance.MovConfigueService.MyExecPath +
                                                 "\\" + "encode.jpc");
                if (buf != null)
                {
                    ACM4500Protocol.UwvdataPool.Add(buf, MovDataType.IMAGE);
                    await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame,
                    (BaseMetroDialog)App.Current.MainWindow.Resources["SendImgDialog"]);
                }
                else
                {
                    var md = new MetroDialogSettings();
                    md.AffirmativeButtonText = "确定";
                    await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "发送失败",
                        "未成功创建压缩图像数据", MessageDialogStyle.Affirmative, md);
                }
            }
        }

        private async void CloseImg(object sender, RoutedEventArgs e)
        {
            await
                MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame,
                    (BaseMetroDialog) App.Current.MainWindow.Resources["SendImgDialog"]);

        }
    }
}
