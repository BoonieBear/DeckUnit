using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BoonieBear.DeckUnit.ACMP;
using BoonieBear.DeckUnit.Mov4500UI.Core;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using BoonieBear.DeckUnit.Mov4500UI.Helpers;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using MahApps.Metro.Controls.Dialogs;
using BoonieBear.DeckUnit.Mov4500UI.ViewModel;
using System.Windows.Forms;
using System.Windows.Controls;
using Button = System.Windows.Controls.Button;
using Cursors = System.Windows.Input.Cursors;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

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
        #endregion
        public LiveCaptureView()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            if (!UnitCore.Instance.IsWorking)
            {
            ServiceInitial:
                bool ret = UnitCore.Instance.Start();
                if (ret == false)
                {
                    var md = new MetroDialogSettings();
                    md.AffirmativeButtonText = "重新连接";
                    md.NegativeButtonText = "修改系统设置";
                    MessageDialogResult answer = await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "启动通信机失败！",
                        UnitCore.Instance.Error, MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary, md);
                    if (answer == MessageDialogResult.Affirmative)
                    {
                        UnitCore.Instance.EventAggregator.PublishMessage(new GoSettingNavigation());
                    }
                    else
                    {
                        if (!UnitCore.Instance.Start())
                            goto ServiceInitial;
                    }
                }
                
            }*/
        }
        public void UpdateRecvStatus()
        {

        }

        //将所有要发送的信息填入chartbox中，靠左对齐
        private void AddSendMsgToChart()
        {
            var title = new Span();
            Image img;
            if (UnitCore.Instance.WorkMode == MonitorMode.SHIP)
            {
                img = new Image//母船的图片
                {
                    Source = ResourcesHelper.LoadBitmapFromResource("Assets\\Logo.jpg"),
                    Width = 40,
                    Height = 40,
                };
            }
            else
            {
                img = new Image//潜器的图片
                {
                    Source = ResourcesHelper.LoadBitmapFromResource("Assets\\Logo.jpg"),
                    Width = 40,
                    Height = 40,
                };
            }
            title.Inlines.Add(img);
            var run = new Run("  ("+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+")")
            {
                FontSize = 24,
                Foreground = Brushes.Green
            };
            title.Inlines.Add(run);
            var titlep = new Paragraph(title);
            titlep.TextAlignment = TextAlignment.Left;
            MessageDocument.Blocks.Add(titlep);
            run = new Run(SendMessageBox.Text);
            p = new Paragraph(run);
            p.TextAlignment = TextAlignment.Left;
            MessageDocument.Blocks.Add(p);
            MessageRichTextBox.ScrollToEnd();
            SendMessageBox.Text = "";
        }
        //将收到的信息填入chartbox中，靠右对齐
        private void AppendRecvInfo(ModuleType type, string msg, Image img)
        {

            var title = new Span();
            Image titleimg;
            if (UnitCore.Instance.WorkMode == MonitorMode.SHIP)
            {
                titleimg = new Image//潜器的图片
                {
                    Source = ResourcesHelper.LoadBitmapFromResource("Assets\\Logo.jpg"),
                    Width = 40,
                    Height = 40,
                };

            }
            else
            {
                titleimg = new Image//潜器的图片
                {
                    Source = ResourcesHelper.LoadBitmapFromResource("Assets\\Logo.jpg"),
                    Width = 40,
                    Height = 40,
                };
            }

            title.Inlines.Add(titleimg);
            var run = new Run("  ("+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+")")
            {
                FontSize = 24,
                Foreground = Brushes.Green
            };
            title.Inlines.Add(run);
            var titlep = new Paragraph(title);
            titlep.TextAlignment = TextAlignment.Right;
            MessageDocument.Blocks.Add(titlep);
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


        


    }
}
