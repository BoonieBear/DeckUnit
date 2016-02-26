using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BoonieBear.DeckUnit.ACMP;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.XtraPrinting.Native;
using ImageProc;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using TinyMetroWpfLibrary.Controller;
using MahApps.Metro.Controls.Dialogs;
using BoonieBear.DeckUnit.Mov4500UI.Core;
using BoonieBear.DeckUnit.Mov4500UI.ViewModel;
using TinyMetroWpfLibrary.EventAggregation;
using TreeHelper = BoonieBear.DeckUnit.Mov4500UI.Helpers.TreeHelper;
using System.IO;
using System.Windows.Threading;
using System.Windows.Media;
using Image = System.Drawing.Image;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace BoonieBear.DeckUnit.Mov4500UI.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainFrame
    {
        private string selectimgpath;
        private bool isdrag = false;
        private double factor = 1.0F;
        private System.Windows.Point offsetPoint;
        [DllImport("user32.dll")]
        private extern static bool SwapMouseButton(bool fSwap);
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
            SwapMouseButton(false);//switch back
            
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
            var cropimgBox = newdialog.FindChild<System.Windows.Controls.Image>("cropimg");
            var checkBox = newdialog.FindChild<System.Windows.Controls.CheckBox>("CropImgChk");
            var container = newdialog.FindChild<System.Windows.Controls.Viewbox>("vb");
            var Section = newdialog.FindChild<System.Windows.Controls.Button>("CropSection");
            OpenFileDialog OpenImgFile = new OpenFileDialog();
            OpenImgFile.Filter="图片文件(*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp";
            if (OpenImgFile.ShowDialog() == true)
            {
                try
                {
                    selectimgpath = OpenImgFile.FileName;
                    imgBox.Source = new BitmapImage(new Uri(selectimgpath, UriKind.Absolute));
                    if (imgBox.Source.Width < 512 || imgBox.Source.Height < 512)
                    {
                        checkBox.Visibility = Visibility.Hidden;
                        cropimgBox.Visibility = Visibility.Hidden;

                    }
                    else
                    {
                        checkBox.Visibility = Visibility.Visible;
                        
                    }
                    checkBox.IsChecked = false;
                }
                catch (Exception MyEx)
                {
                    UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(MyEx, LogType.Both));
                }
            }
            container.Width = 800;
            container.Height = 636;
            Image img = WpfImageSourceToBitmap((BitmapSource)imgBox.Source);

            if (checkBox.IsChecked == true)
            {
                if ((container.ActualWidth / (float)img.Width) < (container.ActualHeight / (float)img.Height))
                    factor = container.ActualWidth / (float)img.Width;
                else
                    factor = container.ActualHeight / (float)img.Height;
                if (factor > 1)
                    factor = 1;

                var offset = Section.TranslatePoint(new System.Windows.Point(0, 0), imgBox);
                if (offset.X < 0 || offset.X + 256 > imgBox.ActualWidth || offset.Y < 0 || offset.Y + 256 > imgBox.ActualHeight)
                    return;
                cropimgBox.Source = BitmapToImageSource(IMGTool.CutImage(img, (int)(offset.X), (int)(offset.Y), (int)(256 / factor), (int)(256 / factor)));
            }
        }
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        private System.Windows.Media.ImageSource BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            IntPtr ptr = bitmap.GetHbitmap();
            System.Windows.Media.ImageSource result =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            //release resource
            DeleteObject(ptr);

            return result;
        }
        private System.Drawing.Bitmap WpfImageSourceToBitmap(BitmapSource s)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(s.PixelWidth, s.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            s.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }
        private async void SendImg(object sender, RoutedEventArgs e)
        {
            var newdialog = (BaseMetroDialog)App.Current.MainWindow.Resources["SendImgDialog"];
            var imgBox = newdialog.FindChild<System.Windows.Controls.Image>("SelectImageContainer");
            var checkBox = newdialog.FindChild<System.Windows.Controls.CheckBox>("CropImgChk");
            var cropimgBox = newdialog.FindChild<System.Windows.Controls.Image>("cropimg");
            if(imgBox.Source==null)
                return;
            var img = new System.Windows.Controls.Image();
            bool bload = false;
            if (checkBox.IsChecked == true)
            {
                img.Source = cropimgBox.Source;
                UnitCore.Instance.AddImgHandle(img);
                bload = Jp2KConverter.LoadImage(WpfImageSourceToBitmap((BitmapSource)cropimgBox.Source));
            }
            else
            {
                img.Source = imgBox.Source;
                UnitCore.Instance.AddImgHandle(img);
                bload = Jp2KConverter.LoadImage(selectimgpath);
            }
            if (bload)
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

        

        
        private void vb1_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isdrag = true;
        }

        private void vb1_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isdrag = false;
        }

        private void vb1_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isdrag) return;
            var control = (sender as UIElement);
            var newdialog = (BaseMetroDialog)App.Current.MainWindow.Resources["SendImgDialog"];
            var container = newdialog.FindChild<System.Windows.Controls.Canvas>("container");
            var imgBox = newdialog.FindChild<System.Windows.Controls.Image>("SelectImageContainer");
            Image img = WpfImageSourceToBitmap((BitmapSource)imgBox.Source);
            var cropimgBox = newdialog.FindChild<System.Windows.Controls.Image>("cropimg");
            var checkBox = newdialog.FindChild<System.Windows.Controls.CheckBox>("CropImgChk");
            var Section = newdialog.FindChild<System.Windows.Controls.Button>("CropSection");
            var view = newdialog.FindChild<System.Windows.Controls.Viewbox>("vb");
            control.SetValue(Canvas.LeftProperty, e.GetPosition(container).X - control.DesiredSize.Width / 2);
            control.SetValue(Canvas.TopProperty, e.GetPosition(container).Y - control.DesiredSize.Height / 2);

            if (checkBox.IsChecked == true)
            {
                //var offset = VisualTreeHelper.GetOffset(control);
                var offset = Section.TranslatePoint(new System.Windows.Point(0, 0), imgBox);
                //cropimgBox.Source = RenderVisaulToBitmap(Section, 256, 256);
                if (offset.X < 0 || offset.X + 256 > imgBox.ActualWidth || offset.Y < 0 || offset.Y + 256 > imgBox.ActualHeight)
                    return;
                //offset don`t have been scaled so start point do not need to moved 
                cropimgBox.Source = BitmapToImageSource(IMGTool.CutImage(img, (int)(offset.X), (int)(offset.Y), (int)(256 / factor), (int)(256 / factor)));
            }
        }
        private void vb1_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var newdialog = (BaseMetroDialog)App.Current.MainWindow.Resources["SendImgDialog"];
            var container = newdialog.FindChild<System.Windows.Controls.Viewbox>("vb");
            var imgBox = newdialog.FindChild<System.Windows.Controls.Image>("SelectImageContainer");
            if (e.Delta > 0)
            {
                container.Width = container.Width * 0.9;
                container.Height = container.Height * 0.9;
                if (container.Width < 512)
                    container.Width = 512;
                if (container.Height < 512)
                    container.Height = 512;
            }
            if (e.Delta < 0)
            {
                container.Width = container.Width * 1.1;
                container.Height = container.Height * 1.1;
            }
            Image img = WpfImageSourceToBitmap((BitmapSource)imgBox.Source);
            if ((container.ActualWidth / (float)img.Width) < (container.ActualHeight / (float)img.Height))
                factor = container.ActualWidth / (float)img.Width;
            else
                factor = container.ActualHeight / (float)img.Height;
            if (factor > 1)
                factor = 1;
        }

        private void CropImgChk_Checked(object sender, RoutedEventArgs e)
        {
            
            var newdialog = (BaseMetroDialog)App.Current.MainWindow.Resources["SendImgDialog"];
            var view = newdialog.FindChild<System.Windows.Controls.Viewbox>("vb");
            var view1 = newdialog.FindChild<System.Windows.Controls.Viewbox>("vb1");
            var container = newdialog.FindChild<System.Windows.Controls.Canvas>("container");
            var cropimgBox = newdialog.FindChild<System.Windows.Controls.Image>("cropimg");
            
            cropimgBox.Visibility = Visibility.Visible;
            container.Visibility = Visibility.Visible;
            var imgBox = newdialog.FindChild<System.Windows.Controls.Image>("SelectImageContainer");
            var Section = newdialog.FindChild<System.Windows.Controls.Button>("CropSection");
            Image img = WpfImageSourceToBitmap((BitmapSource)imgBox.Source);
            if ((view.ActualWidth/ (float)img.Width) < (view.ActualHeight / (float)img.Height))
                factor = view.ActualWidth / (float)img.Width;
            else
                factor = view.ActualHeight / (float)img.Height;
            if (factor > 1)
                factor = 1;
            var offset = Section.TranslatePoint(new System.Windows.Point(0, 0), imgBox);
            if (offset.X < 0 || offset.X + 256 > imgBox.ActualWidth || offset.Y < 0 || offset.Y + 256 > imgBox.ActualHeight)
                return;
            cropimgBox.Source = BitmapToImageSource(IMGTool.CutImage(img, (int)(offset.X), (int)(offset.Y), (int)(256 / factor), (int)(256 / factor)));
        }
        private System.Windows.Point GetPosition(Visual item,Visual parent)
        {
            var transformToAncestor = item.TransformToAncestor(parent);

            var position = transformToAncestor.Transform(new System.Windows.Point(0, 0));

            return position;
        }
        private void CropImgChk_Unchecked(object sender, RoutedEventArgs e)
        {
            var newdialog = (BaseMetroDialog)App.Current.MainWindow.Resources["SendImgDialog"];
            var container = newdialog.FindChild<System.Windows.Controls.Canvas>("container");
            var cropimgBox = newdialog.FindChild<System.Windows.Controls.Image>("cropimg");
            container.Visibility = Visibility.Hidden;
            cropimgBox.Visibility = Visibility.Hidden;
        }

    }
}
