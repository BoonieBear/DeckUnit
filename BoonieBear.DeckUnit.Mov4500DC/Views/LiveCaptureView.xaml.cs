using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using BoonieBear.DeckUnit.ACMP;
using BoonieBear.DeckUnit.Mov4500UI.Core;
using BoonieBear.DeckUnit.Mov4500UI.Helpers;
using System.Windows.Controls;
using TinyMetroWpfLibrary.EventAggregation;
using Button = System.Windows.Controls.Button;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using BoonieBear.DeckUnit.WaveBox;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using MahApps.Metro.Controls.Dialogs;
using BoonieBear.DeckUnit.Mov4500UI.ViewModel;
namespace BoonieBear.DeckUnit.Mov4500UI.Views
{
    /// <summary>
    /// LiveCaptureView.xaml 的交互逻辑
    /// </summary>
    public partial class LiveCaptureView : Page
    {
        #region 变量区域
        //选择图片的文件名
        private string _fileName = string.Empty;
        private Paragraph p = null;
        //记录Span
        Dictionary<int, Span> spans = new Dictionary<int, Span>(0);
        List<Paragraph> parasList = new List<Paragraph>(0);
        private int index = 0;
        private bool StartRecode = false;
        #endregion
        public LiveCaptureView()
        {
            InitializeComponent();
            if (!WaveControl.IsInit)
                WaveControl.Initailize();
            WaveControl.PlayMode = WaveControl.Mode.Both;
            WaveControl.AddRecDoneHandle(RecHandle);
            UnitCore.Instance.liveBox = WaveControl;
        }

        private void RecHandle(byte[] bufBytes)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                //WaveControl.Display(bufBytes);
                if (StartRecode)
                    VoiceBar.Value = bufBytes.Sum((s)=>(int)s)*100/(2048*255);
            }) );
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await TryConnnect();
            if (UnitCore.Instance.IsWorking)
            {
                
            }
        }

        private static async Task TryConnnect()
        {
            if (!UnitCore.Instance.IsWorking)
            {
                while (UnitCore.Instance.Start()== false)
                {
                    var md = new MetroDialogSettings();
                    md.AffirmativeButtonText = "重试连接";
                    md.NegativeButtonText = "修改设置";
                    md.FirstAuxiliaryButtonText = "取消";
                    md.ColorScheme = MetroDialogColorScheme.Accented;
                    MessageDialogResult answer =
                        await
                            MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame,
                                "启动通信机失败！",
                                UnitCore.Instance.Error, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, md);
                    if (answer == MessageDialogResult.Negative)
                    {
                        UnitCore.Instance.EventAggregator.PublishMessage(new GoSettingNavigation());
                    }
                    else if (answer == MessageDialogResult.FirstAuxiliary)
                    {
                        break;
                    }
                    else
                    {
                        if (UnitCore.Instance.Start())
                            break;
                    }
                }
            }
        }

        public void UpdateRecvStatus()
        {

        }

        //将要发送的文字信息填入chartbox中，靠左对齐
        private void AddSendMsgToChart()
        {
            Run run;
            SetTiltle(true);
            run = new Run(SendMessageBox.Text);
            p = new Paragraph(run);
            p.TextAlignment = TextAlignment.Left;
            MessageDocument.Blocks.Add(p);
            MessageRichTextBox.ScrollToEnd();
            SendMessageBox.Text = "";
        }

        //向chartbox中加入信息标题， 发送信息靠左，接收信息靠右
        private void SetTiltle(bool sender)
        {
            var title = new Span();
            Image img;
            if (UnitCore.Instance.WorkMode == MonitorMode.SHIP)
            {
                img = new Image //母船的图片
                {
                    Source = ResourcesHelper.LoadBitmapFromResource("Assets\\Logo.jpg"),
                    Width = 40,
                    Height = 40,
                };
            }
            else
            {
                img = new Image //潜器的图片
                {
                    Source = ResourcesHelper.LoadBitmapFromResource("Assets\\Logo.jpg"),
                    Width = 40,
                    Height = 40,
                };
            }
            title.Inlines.Add(img);
            var run = new Run("  (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ")")
            {
                FontSize = 24,
                Foreground = Brushes.Green
            };
            title.Inlines.Add(run);
            var titlep = new Paragraph(title);
            if (sender)
                titlep.TextAlignment = TextAlignment.Left;
            else
            {
                titlep.TextAlignment = TextAlignment.Right;
            }
            MessageDocument.Blocks.Add(titlep);
        }

        private void AddSendFHToChart(string msg)
        {
            SetTiltle(true);
            if (msg == null)
                return;
            var run = new Run("(" + "跳频" + ")" + msg);
            run.FontSize = 22;
            var chartmsg = new Paragraph(run);
            chartmsg.TextAlignment = TextAlignment.Left;
            chartmsg.LineHeight = 24;
            MessageDocument.Blocks.Add(chartmsg);
        }

        private void AddSendImgToChart(Image img)
        {
            SetTiltle(true);
            var p = new Paragraph();
            p.Inlines.Add(img);
            p.TextAlignment = TextAlignment.Left;
            MessageDocument.Blocks.Add(p);
        }
        //将收到的信息填入chartbox中，靠右对齐
        private void AppendRecvInfo(ModuleType type, string msg, Image img)
        {
            SetTiltle(false);
            Run run;
            if (type == ModuleType.MFSK)
            {
                if(msg==null)
                    return;
                run = new Run(msg);
                run.FontSize = 22;
                var chartmsg = new Paragraph(run);
                chartmsg.TextAlignment = TextAlignment.Right;
                chartmsg.LineHeight = 24;
                MessageDocument.Blocks.Add(chartmsg);
            }
            else if(type == ModuleType.FH)
            {
                if(msg==null)
                    return;
                run = new Run("("+"跳频"+")"+msg);
                run.FontSize = 22;
                var chartmsg = new Paragraph(run);
                chartmsg.TextAlignment = TextAlignment.Right;
                chartmsg.LineHeight = 24;
                MessageDocument.Blocks.Add(chartmsg);
            }
            else if (type == ModuleType.MPSK)
            {
                if (msg != null)
                {
                    run = new Run(msg);
                    run.FontSize = 22;
                    var chartmsg = new Paragraph(run);
                    chartmsg.TextAlignment = TextAlignment.Right;
                    chartmsg.LineHeight = 24;
                    MessageDocument.Blocks.Add(chartmsg);
                }
                if (img != null )
                {
                    var p = new Paragraph();
                    p.Inlines.Add(img);
                    p.TextAlignment = TextAlignment.Right;
                    MessageDocument.Blocks.Add(p);
                }
            }

            MessageRichTextBox.ScrollToEnd();
        }
        //将需要发送的内容填入发送对话框
        private void AppendSendInfo(string msg)
        {
            if (msg != null)
                SendMessageBox.Text += msg;
            SendMessageBox.ScrollToEnd();
        }
       
        private void SendMessageBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SendMessageBox.Text.Length>20)
            {
                SendMessageBox.Text = SendMessageBox.Text.Substring(0,20);
                SendMessageBox.CaretIndex = 20;
            }
            int left = 20 - SendMessageBox.Text.Length;
            LeftSize.Text = "(还可继续输入"+left+"个字)";
            if(SendMessageBox.Text.Length>0)
                SendBtn.Visibility = Visibility.Visible;
            else
                SendBtn.Visibility = Visibility.Hidden;
        }

        private void SendMessageBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
             if (e.Key== Key.Return&&SendMessageBox.Text!="")
                AddSendMsgToChart();
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if(SendMessageBox.Text!="")
                AddSendMsgToChart();
        }

        private void SendMessageBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SendMessageBox.Background = (SolidColorBrush)FindResource("Home_LiveBackground");
        }

        private void SendMessageBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(SendMessageBox.Text.Length==0)
                SendMessageBox.Background = (SolidColorBrush) FindResource("Home_ShareBackground");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            fanpanel.IsOpen = true;
        }


        private void Page_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in fanpanel.Children)
            {
                Button btn = item as Button;
                //在此加入点击某些按钮时的特殊效果
                if (btn.Name == "" && btn.IsMouseOver)
                {
                    return;
                }

                if ((btn != null && btn.IsMouseOver))
                {
                    return;
                }
            }
            fanpanel.IsOpen = false;
        }

        private void SendSSBBtn_Click(object sender, RoutedEventArgs e)
        {
            SendSSBBtn.Visibility = Visibility.Hidden;
            BackToEditBtn.Visibility = Visibility.Visible;
            SSBToolTip.Visibility = Visibility.Visible;
            VoiceBar.Visibility = Visibility.Visible;
            LeftSize.Visibility = Visibility.Hidden;
        }

        private void VoiceBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SSBToolTip.Content = "释放后发送语音";
            StartRecode = true;
        }

        private void VoiceBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SSBToolTip.Content = "按住说话";
            StartRecode = false;
        }
        //和释放动作效果一样
        private void VoiceBar_MouseLeave(object sender, MouseEventArgs e)
        {
            SSBToolTip.Content = "按住说话";
            StartRecode = false;
        }
        private void BackToEditBtn_Click(object sender, RoutedEventArgs e)
        {
            SendSSBBtn.Visibility = Visibility.Visible;
            BackToEditBtn.Visibility = Visibility.Hidden;
            SSBToolTip.Visibility = Visibility.Hidden;
            VoiceBar.Visibility = Visibility.Hidden;
            LeftSize.Visibility = Visibility.Visible;
        }
    }
}
