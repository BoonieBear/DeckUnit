using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BoonieBear.DeckUnit.ACMP;
using BoonieBear.DeckUnit.Mov4500Conf;
using BoonieBear.DeckUnit.Mov4500UI.Core;
using BoonieBear.DeckUnit.Mov4500UI.Helpers;
using System.Windows.Controls;
using BoonieBear.DeckUnit.TraceFileService;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Core;
using HelixToolkit.Wpf;
using ImageProc;
using TinyMetroWpfLibrary.EventAggregation;
using Button = System.Windows.Controls.Button;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using BoonieBear.DeckUnit.WaveBox;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using MahApps.Metro.Controls.Dialogs;
using BoonieBear.DeckUnit.Mov4500UI.ViewModel;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using MahApps.Metro.Controls;
using DevExpress.XtraCharts;
using BoonieBear.DeckUnit.Comm.GPIO;
namespace BoonieBear.DeckUnit.Mov4500UI.Views
{
    /// <summary>
    /// LiveCaptureView.xaml 的交互逻辑
    /// </summary>
    public partial class LiveCaptureView : Page
    {

        [DllImport("user32.dll")]
        private extern static bool SwapMouseButton(bool fSwap);
        #region 变量区域
		bool VoiceBar_MouseLeftButton = false;//记录界面是否在录音
        bool VoiceBarIsable = true;
        GPIOService GPIO = new GPIOService();
        private bool _isPressed = false;
        private Point _sourcePoint;
        //选择图片的文件名
        private string _fileName = string.Empty;
        private Paragraph p = null;
        //记录Span
        Dictionary<int, Span> spans = new Dictionary<int, Span>(0);
        List<Paragraph> parasList = new List<Paragraph>(0);
        Dictionary<string, Viewbox> ImgViews = new Dictionary<string, Viewbox>(0);//收到图像的集合
        DispatcherTimer ImgWatcher= null;
        DispatcherTimer GPIOWatcher = null;
        private int index = 0;
        private bool isRecording = false;
        private short[] volumnbuffer = new short[400];//half of wavecontrol recording buffer
        private readonly Dispatcher dispatcher;
        #endregion
        public LiveCaptureView()
        {
            InitializeComponent();            
            GPIO.Initialize_SUSI();
            WaveControl.Initailize();
            WaveControl.AddRecDoneHandle(RecHandle);
            WaveControl.StartPlaying();
            this.dispatcher = Dispatcher.CurrentDispatcher;
            UnitCore.Instance.LiveHandle = AppendRecvInfo;
            UnitCore.Instance.AddFHHandle = AddSendFHToChart;
            UnitCore.Instance.AddImgHandle = AddSendImgToChart;
        }

        void Tick(object sender, EventArgs e)
        {
            try
            {
               if(UnitCore.Instance.NetCore.IsTCPWorking)
               {
                   if (VoiceBar_MouseLeftButton==false)//界面不在录音
                   {
                       GPIO.IORead();
                       if ((GPIO.StatusMask & 0x00000002) == 2)//GPIO口接的是第二个，未录音时状态是0b11111101
                       {
                           if (!isRecording)
                           {
                               Recording(true);
                           }

                       }
                       else
                       {
                           if (isRecording)
                           {
                               Recording(false);
                           }

                       }

                   }
                   

               }
               else
               {
                   if (isRecording)
                   {
                       Recording(false);
                   }
                   

               }
                

            }
            catch (Exception ex)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent(ex.Message, LogType.Both));
            }


        }

        private void RecHandle(byte[] bufBytes)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                UnitCore.Instance.MovTraceService.Save("XMTVOICE", bufBytes);
                try
                {
                    if (isRecording)
                    {
                        if (UnitCore.Instance.NetCore.IsTCPWorking)
                        {
                            UnitCore.Instance.NetCore.Send((int)ModuleType.SSB, bufBytes);
                        }
                        VoiceBar.Value = UpdateVolumn(bufBytes);
                    }
                                                
                    //WaveControl.Display(bufBytes);
                    
                }
                catch (Exception ex)
                {
                    UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent(ex.Message, LogType.Both));
                }
                
            }));
            
            
        }
        /// <summary>
        /// 计算音量，输出离散化
        /// </summary>
        /// <param name="bufBytes"></param>
        private int UpdateVolumn(byte[] bufBytes)
        {
            Buffer.BlockCopy(bufBytes, 0, volumnbuffer, 0, bufBytes.Length);
            // 将 buffer 内容取出，进行平方和运算
            double v = 0;
            int newvolumn = 0;
            for (int i = 0; i < volumnbuffer.Length; i++)
            {
                v += volumnbuffer[i] * volumnbuffer[i];
            }
            // 平方和除以数据总长度，得到音量大小。
            double mean = v / (double)volumnbuffer.Length;
            var volume = 10 * Math.Log10(mean);
            if (volume < 10)
                volume = 5;
            else if ( volume < 30)
                newvolumn = 0;
            else if (volume > 30 && volume < 40)
                newvolumn = 15;
            else if (volume > 40 && volume < 50)
                newvolumn = 35;
            else if (volume > 50 && volume < 60)
                newvolumn = 55;
            else if (volume > 60 && volume < 70)
                newvolumn = 70;
            else if (volume > 70 && volume < 80)
                newvolumn = 85;
            else if (volume > 80)
                newvolumn = 100;

            return newvolumn;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            if (UnitCore.Instance.WorkMode == MonitorMode.SHIP)
            {
                LiveTitle.TitleImageSource = ResourcesHelper.LoadBitmapFromResource("Assets\\shipsnapshot.png");
                var LastComboItem = EmitSelBox.Items[3] as ComboBoxItem;
                LastComboItem.Visibility = Visibility.Visible;
            }
            else
            {
                LiveTitle.TitleImageSource = ResourcesHelper.LoadBitmapFromResource("Assets\\Logo_nbg.png");
                var LastComboItem = EmitSelBox.Items[3] as ComboBoxItem;
                LastComboItem.Visibility = Visibility.Hidden;
            }
            try
            {
                if (ShipD.Content == null)
                {
                    string modelpath = MovConf.GetInstance().MyExecPath + "\\" + "Assets\\Ship.3DS";
                    ShipD.Content = await LoadAsync(modelpath, false);
                    if (ShipD.Content == null)
                        throw new Exception("加载母船组件失败！");
                }
                if (MovD.Content == null)
                {
                    string modelpath = MovConf.GetInstance().MyExecPath + "\\" + "Assets\\jl.obj";
                    MovD.Content = await LoadAsync(modelpath, false);
                    if (MovD.Content == null)
                        throw new Exception("加载潜器组件失败！");
                }
                PosViewport3D.CameraController.ChangeDirection(new Vector3D(-12000, 0, -6000), 1000);
            }
            catch (Exception ex)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent(ex.Message, LogType.Both));
                
            }
            SwapMouseButton(false);

            

            await TryConnnect();
            if (UnitCore.Instance.NetCore.IsTCPWorking)
            {
                SetupDSP();
                NolinkBlock.Visibility = Visibility.Hidden;
            }
            UnitCore.Instance.Wave = WaveControl;
            if (UnitCore.Instance.WorkMode == MonitorMode.SUBMARINE)
            {
                MultiView.HideControlButtons();
                MultiView.SelectedIndex = 1;
            }
            else
            {
                MultiView.ShowControlButtons();
                MultiView.SelectedIndex = 0;
            }
            if (ImgWatcher == null)
                ImgWatcher = new DispatcherTimer(TimeSpan.FromSeconds(5), DispatcherPriority.Normal, RecvImgWatcher,
                    dispatcher);
            ImgWatcher.Start();
            if(GPIOWatcher==null)
                GPIOWatcher = new DispatcherTimer(TimeSpan.FromMilliseconds(250), DispatcherPriority.Normal, Tick, Dispatcher.CurrentDispatcher);
            GPIOWatcher.Start();


            
        }

        private void RecvImgWatcher(object sender, EventArgs e)
        {
            var imgpath = TraceFile.GetInstance().TracePath + @"\RECVPSK";
            if(!Directory.Exists(imgpath))
                return;
            var filenames = Directory.GetFiles(imgpath,"*.jpg");
            var er = ImgViews.GetEnumerator();
            
            List<string> Viewboxes= new List<string>();

            while (er.MoveNext())
            {
                if (filenames.Any(s => { return s == er.Current.Key; }) == false)
                {
                    Viewboxes.Add(er.Current.Key);
                }
            }
            foreach (var key in Viewboxes)
            {
                var box = ImgViews[key];
                ImagePanel.Children.Remove(box);
                ImgViews.Remove(key);
            }

            
            Viewboxes.Clear();
            foreach (var filename in filenames)
            {
                if (ImgViews.ContainsKey(filename) == false)
                {
                    Viewboxes.Add(filename);
                }
            }
            foreach (var key in Viewboxes)
            {
                var img = new Image();
                img.Source = new BitmapImage(new Uri(key));
                var vew = new Viewbox();
                vew.Child = img;
                vew.MouseDown += vew_MouseDown;
                vew.ToolTip = key;
                ImagePanel.Children.Insert(0,vew);
                ImgViews.Add(key,vew);
            }
            if (ImgViews.Count == 0)
            {
                MultiView.SelectedIndex = 1;
                MultiView.HideControlButtons();
            }
        }

        private void vew_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var src = sender as Viewbox;
                if (src != null)
                {
                    var img = src.Child as Image;
                    if (img != null)
                    {
                        Process ps = new Process();
                        ps.StartInfo.FileName = "explorer.exe";
                        ps.StartInfo.Arguments = img.Source.ToString();
                        ps.Start();
                    }
                }
            }
        }
        private async Task<Model3DGroup> LoadAsync(string model3DPath, bool freeze)
        {
            return await Task.Factory.StartNew(() =>
            {
                var mi = new ModelImporter();
                if (freeze)
                {
                    // Alt 1. - freeze the model 
                    return mi.Load(model3DPath, null, true);
                }

                // Alt. 2 - create the model on the UI dispatcher
                return mi.Load(model3DPath, this.dispatcher);

            });
        }
        private static async Task TryConnnect()
        {
            while (UnitCore.Instance.NetCore==null)
            {
                Thread.Sleep(200);
            }
            if (UnitCore.Instance.NetCore!=null&&!UnitCore.Instance.NetCore.IsTCPWorking)
            {
                while (UnitCore.Instance.NetCore.StartTCPService() == false)
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
                        break;
                    }
                    else if (answer == MessageDialogResult.FirstAuxiliary)
                    {
                        break;
                    }
                    else
                    {
                        if (UnitCore.Instance.NetCore.StartTCPService())
                        {
                            
                            break;
                        }
                            
                    }
                }
            }
        }

        private static async void SetupDSP()
        {
            if (UnitCore.Instance.NetCore.IsTCPWorking)
            {
                string gain = UnitCore.Instance.MovConfigueService.GetGain();
                string amp = UnitCore.Instance.MovConfigueService.GetXmtAmp();
                string channel = UnitCore.Instance.MovConfigueService.GetXmtChannel();
                MonitorMode mode = UnitCore.Instance.MovConfigueService.GetMode();
                string dspmode = (mode == MonitorMode.SHIP) ? "2" : "1";
                var cmd = "synseq " + dspmode;
                await UnitCore.Instance.NetCore.SendConsoleCMD(cmd);
                await TaskEx.Delay(200);
                cmd = "channel " + channel + " -w";
                await UnitCore.Instance.NetCore.SendConsoleCMD(cmd);
                LogHelper.WriteLog("发射换能器设置为"+channel);
                await TaskEx.Delay(TimeSpan.FromMilliseconds(200));
                cmd = "opent " + channel + " -w";
                await UnitCore.Instance.NetCore.SendConsoleCMD(cmd);
                LogHelper.WriteLog("接收换能器设置为" + channel);
                await TaskEx.Delay(TimeSpan.FromMilliseconds(200));
                cmd = "a " + amp;
                await UnitCore.Instance.NetCore.SendConsoleCMD(cmd);
                LogHelper.WriteLog("发射幅度设置为" + amp);
                await TaskEx.Delay(TimeSpan.FromMilliseconds(200));
                if (UnitCore.Instance.MovConfigueService.GetGMode() == MonitorGMode.HAND)
                {
                    cmd = "gm 1";
                    await UnitCore.Instance.NetCore.SendConsoleCMD(cmd);
                    cmd = "gm "+gain;
                    await UnitCore.Instance.NetCore.SendConsoleCMD(cmd);
                    LogHelper.WriteLog("接收增益设置为" + gain);
                }
                else
                {
                    cmd = "gm 2";
                    await UnitCore.Instance.NetCore.SendConsoleCMD(cmd);
                    LogHelper.WriteLog("接收增益设置为自动增益模式");

                }
                
                await TaskEx.Delay(TimeSpan.FromMilliseconds(200));
                DateTime dt = DateTime.Now;
                cmd = "date " + dt.Year.ToString("D4") + " " + dt.Month.ToString("D2") + " " + dt.Day.ToString("D2") +
                      " " + dt.Hour.ToString("D2") + " " + dt.Minute.ToString("D2") + " " + dt.Second.ToString("D2");
                await UnitCore.Instance.NetCore.SendConsoleCMD(cmd);
            }
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
            if(UnitCore.Instance.WorkMode== MonitorMode.SUBMARINE)
                ACM4500Protocol.UwvdataPool.Add(Encoding.Default.GetBytes(SendMessageBox.Text),MovDataType.WORD);
            else
                ACM4500Protocol.ShipdataPool.Add(Encoding.Default.GetBytes(SendMessageBox.Text), MovDataType.WORD);
            if (UnitCore.Instance.WorkMode == MonitorMode.SHIP)
            {

                UnitCore.Instance.MovTraceService.Save("Chart", "（母船）" + SendMessageBox.Text);
                MainFrameViewModel.pMainFrame.MsgLog.Add(DateTime.Now.ToShortTimeString() + ":" +
                                                                         "（母船）" + SendMessageBox.Text);
            }
            else
            {

                UnitCore.Instance.MovTraceService.Save("Chart", "（潜器）" + SendMessageBox.Text);
                MainFrameViewModel.pMainFrame.MsgLog.Add(DateTime.Now.ToShortTimeString() + ":" +
                                                                         "（潜器）" + SendMessageBox.Text);
            }
            SendMessageBox.Text = "";
        }

        //向chartbox中加入信息标题， 发送信息靠左，接收信息靠右
        private void SetTiltle(bool sender)
        {
            var title = new Span();
            Image img,shipImg,movImage;
            shipImg = new Image //母船的图片
            {
                Source = ResourcesHelper.LoadBitmapFromResource("Assets\\shipsnapshot.png"),
                Width = 48,
                Height = 48,
            };
            movImage = new Image //潜器的图片
            {
                Source = ResourcesHelper.LoadBitmapFromResource("Assets\\Logo_nbg.png"),
                Width = 48,
                Height = 48,
            };
            if (UnitCore.Instance.WorkMode == MonitorMode.SHIP)
            {
                if (sender)
                    img = shipImg;
                else
                {
                    img = movImage;
                }
            }
            else
            {
                if (sender)
                    img = movImage;
                else
                {
                    img = shipImg;
                }
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
            MessageRichTextBox.ScrollToEnd();
            if (UnitCore.Instance.WorkMode == MonitorMode.SHIP)
            {
                UnitCore.Instance.MovTraceService.Save("FH", "（母船）" + msg);
                UnitCore.Instance.MovTraceService.Save("Chart", "（母船-跳频）" + msg);
            }
            else
            {
                UnitCore.Instance.MovTraceService.Save("FH", "（潜器）" + msg);
                UnitCore.Instance.MovTraceService.Save("Chart", "（潜器-跳频）" + msg);
            }
        }
        private void AddSendSSBToChart(string msg)
        {
            SetTiltle(true);
            if (msg == null)
                return;
            var run = new Run("(" + "水声摩斯码" + ")" + msg);
            run.FontSize = 22;
            var chartmsg = new Paragraph(run);
            chartmsg.TextAlignment = TextAlignment.Left;
            chartmsg.LineHeight = 24;
            MessageDocument.Blocks.Add(chartmsg);
            MessageRichTextBox.ScrollToEnd();
            UnitCore.Instance.MovTraceService.Save("Chart",  "(" + "水声摩斯码" + ")" + msg);

        }
        private void AddSendImgToChart(Image img)
        {
            SetTiltle(true);
            var p = new Paragraph();
            //var vew = new Viewbox();
            //vew.Child = img;
            p.Inlines.Add(img);
            p.TextAlignment = TextAlignment.Left;
            //vew.HorizontalAlignment = HorizontalAlignment.Left;
            MessageDocument.Blocks.Add(p);
            MessageRichTextBox.ScrollToEnd();
            UnitCore.Instance.MovTraceService.Save("Chart", "（潜器-MPSK）" + img.Source);
        }
        //将收到的信息填入chartbox中，靠右对齐
        private void AppendRecvInfo(ModuleType type, string msg, Image img)
        {
            SetTiltle(false);
            Run run;
            if (type == ModuleType.MFSK)
            {
                if(msg==null)
                {
                    MessageRichTextBox.ScrollToEnd();
                    return;
                }               
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
                    MultiView.SelectedIndex = 0;
                    MultiView.ShowControlButtons();
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
            if (MorseGrid.IsMouseOver)
                return;
           
            MorseRow.Height = new GridLength(0);
        }

        private void SendSSBBtn_Click(object sender, RoutedEventArgs e)
        {
            SendSSBBtn.Visibility = Visibility.Hidden;
            BackToEditBtn.Visibility = Visibility.Visible;
            SSBToolTip.Visibility = Visibility.Visible;
            VoiceBar.Visibility = Visibility.Visible;
            LeftSize.Visibility = Visibility.Hidden;
            SendMessageBox.Visibility = Visibility.Hidden;
        }

        private void VoiceBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           // if (VoiceBarIsable==true)
           // {
                VoiceBar_MouseLeftButton = true;//按钮按下后，界面获取主动权

                if (!isRecording)
                {
                    Recording(true);
                }
                else
                {
                    return;
                }
           // }
            

            
        }
        
        private void Recording(bool start = true)
        {
            try
            {
                if (start)
                {
                    LogHelper.WriteLog("开始录音");
                    if (UnitCore.Instance.NetCore.IsTCPWorking)
                    {
                        UnitCore.Instance.NetCore.SendSSBNon();
                        UnitCore.Instance.NetCore.Send((int)ModuleType.SSB, UnitCore.Instance.Single);
                        LogHelper.WriteLog("开始发送语音");
                        isRecording = true;
                        WaveControl.StartRecording();
                        SSBToolTip.Content = "正在发送语音";
                    }

                }
                else
                {
                    if (isRecording)
                    {
                       // VoiceBarIsable = false;
                        VoiceBar.IsEnabled = false;
                        WaveControl.StopRecoding();
                        isRecording = false;//此处给出停止录音标志，语音线程停止发送，之后再发END包
                        if (UnitCore.Instance.NetCore.IsTCPWorking)
                        {
                            UnitCore.Instance.NetCore.SendSSBEND();
                            LogHelper.WriteLog("结束录音");
                            UnitCore.Instance.MovTraceService.EndSave("XMTVOICE");
                            LogHelper.WriteLog("结束语音发送");

                        }
                        VoiceBarDealy();//留出一段时间给DSP处理单频
                        
                        
                    }

                }
                VoiceBar.Value = 0;

            }
            catch (Exception ex)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent(ex.Message, LogType.Both));
            }
            
            
        }

        private async Task VoiceBarDealy()
        {
            await TaskEx.Delay(500);
          //  VoiceBarIsable = true;
            VoiceBar.IsEnabled = true;
            VoiceBar_MouseLeftButton = false;//界面停止录音后再交出主动权
            SSBToolTip.Content = "按住说话";


        }

        private void VoiceBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
           // if (VoiceBarIsable==true)
          //  {
                if (isRecording)
                {
                    Recording(false);
                }
                
          //  }
            
            
        }
        //和释放动作效果一样
        private void VoiceBar_MouseLeave(object sender, MouseEventArgs e)
        {
            Recording(false);
        }
        private void BackToEditBtn_Click(object sender, RoutedEventArgs e)
        {
            SendSSBBtn.Visibility = Visibility.Visible;
            BackToEditBtn.Visibility = Visibility.Hidden;
            SSBToolTip.Visibility = Visibility.Hidden;
            VoiceBar.Visibility = Visibility.Hidden;
            LeftSize.Visibility = Visibility.Visible;
            SendMessageBox.Visibility = Visibility.Visible;
        }

        private void NorthButton_Click(object sender, RoutedEventArgs e)
        {
            if (PosViewport3D == null || PosViewport3D.CameraController == null)
                return;
            PosViewport3D.CameraController.ChangeDirection(new Vector3D(0, 0, -6000), new Vector3D(-1, 0, 0), 1000);
            
        }

        private void AddMorseBtn_Click(object sender, RoutedEventArgs e)
        {
            MorseRow.Height =new GridLength(120);
        }

        private async void SendMorse(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null&&UnitCore.Instance.NetCore.IsTCPWorking)
            {
                try
                {
                    switch (btn.Content.ToString())
                    {
                    case "收到":
                    case "一切正常":
                        UnitCore.Instance.NetCore.SendSSBNon();
                        UnitCore.Instance.NetCore.Send((int) ModuleType.SSB, UnitCore.Instance.RecvOrOK);
                        UnitCore.Instance.NetCore.SendSSBEND();
                        
                        break;
                    case "询问情况":
                    case "完成阶段工作":
                        UnitCore.Instance.NetCore.SendSSBNon();
                        UnitCore.Instance.NetCore.Send((int)ModuleType.SSB, UnitCore.Instance.AskOrOK);
                        UnitCore.Instance.NetCore.SendSSBEND();
                        break;
                    case "同意":
                    case "请求上浮":
                        UnitCore.Instance.NetCore.SendSSBNon();
                        UnitCore.Instance.NetCore.Send((int)ModuleType.SSB, UnitCore.Instance.AgreeOrReqRise);
                        UnitCore.Instance.NetCore.SendSSBEND();
                        break;
                    case "立即上浮":
                    case "进入应急程序":
                        UnitCore.Instance.NetCore.SendSSBNon();
                        UnitCore.Instance.NetCore.Send((int)ModuleType.SSB, UnitCore.Instance.RiseOrUrgent);
                        UnitCore.Instance.NetCore.SendSSBEND();
                        break;
                    case "不同意":
                        UnitCore.Instance.NetCore.SendSSBNon();
                        UnitCore.Instance.NetCore.Send((int)ModuleType.SSB, UnitCore.Instance.Disg);
                        UnitCore.Instance.NetCore.SendSSBEND();
                        break;
                    case "释放应急浮标":
                        UnitCore.Instance.NetCore.SendSSBNon();
                        UnitCore.Instance.NetCore.Send((int)ModuleType.SSB, UnitCore.Instance.RelBuoy);
                        UnitCore.Instance.NetCore.SendSSBEND();
                        break;
                    default:
                        break;
                    }
                    AddSendSSBToChart(btn.Content.ToString());
                    LogHelper.WriteLog("发送摩斯码" + btn.Content.ToString());
                }
                catch (Exception ex)
                {
                    UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(ex, LogType.Both));
                }

            }
        }

        private void PosViewport3D_MouseEnter(object sender, MouseEventArgs e)
        {
            SwapMouseButton(true);//switch to right button mode to manipulate pos view,For the device which don` t have multi-touch
        }

        private void PosViewport3D_MouseLeave(object sender, MouseEventArgs e)
        {
            SwapMouseButton(false);//switch back
        }

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var flipview = ((FlipView)sender);
            switch (flipview.SelectedIndex)
            {
                case 0:
                    //flipview.BannerText = "高速图像";
                    break;
                case 1:
                     //flipview.BannerText = "潜器数据";
                    break;
                default:
                    break;
            }
        }

        private void PosViewport3D_LostFocus(object sender, RoutedEventArgs e)
        {
            SwapMouseButton(false);//switch back
        }


        private void HeadChoser_Unchecked(object sender, RoutedEventArgs e)
        {
            HeadChoser.Content = "北方为正前";
        }

        private void HeadChoser_Checked(object sender, RoutedEventArgs e)
        {
            HeadChoser.Content = "船艏为正前";
        }

        private void PosViewport3D_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var distance = PosViewport3D.CameraController.CameraPosition.DistanceTo(new Point3D(0,0,0));
            if (distance > 12000&&e.Delta<0)
                e.Handled = true;
            if (distance < 6000&&e.Delta>0)
                e.Handled = true;
        }
    }
}
