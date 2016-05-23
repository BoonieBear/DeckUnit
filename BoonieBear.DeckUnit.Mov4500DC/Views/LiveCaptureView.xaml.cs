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
using BoonieBear.DeckUnit.Comm.PCI;
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
        public static Thread m_pThread_BPsend;
        public Thread m_pMessage_BPsend;
        public int idcount = 0;
        public static int DelayTime = 0;
        DispatcherTimer CountWatcher = null;
        int Count = 0;
        bool IsBPInitialize = false;

        int nAmp = 4004;//录音系数
        Int16[] databuf = new Int16[16000];
        int count = 0;
		bool VoiceBar_MouseLeftButton = false;//记录界面是否在录音
        bool VoiceBarIsable = true;
        GPIOService GPIO = new GPIOService();
        PCIService PCIIO = new PCIService();
        bool RecordIOStatus = false;
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

        public bool GPIOInitial = true;
        public bool PCIInitial = true;
        #endregion
        public LiveCaptureView()
        {
            InitializeComponent();             
            GPIOInitial=GPIO.Initialize_SUSI();
            PCIInitial=PCIIO.Initialize_PCI();
            WaveControl.Initailize();
            WaveControl.AddRecDoneHandle(RecHandle);
            WaveControl.StartPlaying();
            this.dispatcher = Dispatcher.CurrentDispatcher;
            UnitCore.Instance.LiveHandle = AppendRecvInfo;
            UnitCore.Instance.AddFHHandle = AddSendFHToChart;
            UnitCore.Instance.AddImgHandle = AddSendImgToChart;

            LeftSize.Text = "(还可继续输入" + UnitCore.Instance.MFSK_LeftSize + "个字)";
            SendMessageBox.MaxLength = UnitCore.Instance.MFSK_LeftSize;
            
            
           /* if(SendMessageBox.MaxLength==0)
            {
                SendMessageBox.IsReadOnly = true;
            }*/

        }

        void Tick(object sender, EventArgs e)
        {
            try
            {
                if (UnitCore.Instance.PostMsgEvent_Tick.WaitOne(100))
                {
                    UnitCore.Instance.PostMsgEvent_Tick.Reset();                    
                    if (UnitCore.Instance.NetCore.IsTCPWorking)
                    {
                        if (VoiceBar_MouseLeftButton == false)//界面不在录音
                        {
                            if (UnitCore.Instance.MovConfigueService.GetMode() == MonitorMode.SHIP)
                            {
                                if (PCIInitial == true)
                                {
                                    PCIIO.IORead();
                                    if ((PCIIO.portData[0] & 0x01) == 0)//针1电平反向了，按钮按下是低电平
                                    {
                                        RecordIOStatus = true;
                                    }
                                    else
                                    {
                                        RecordIOStatus = false;
                                    }
                                }

                            }
                            else
                            {
                                if (GPIOInitial == true)
                                {
                                    GPIO.IORead();
                                    if ((GPIO.StatusMask & 0x00000002) == 2)//GPIO口接的是第二个，未录音时状态是0b11111101
                                        RecordIOStatus = true;
                                    else
                                        RecordIOStatus = false;
                                }

                            }

                       if (RecordIOStatus)
                       {
                           if (!isRecording)
                           {
                               isRecording = true;
                               App.Current.Dispatcher.Invoke(new Action(() =>
                               {
                                   SendSSBBtn_Click(null, null);
                               }));
                               Recording(true);
                           }

                            }
                            else
                            {
                                if (isRecording)
                                {
                                    App.Current.Dispatcher.Invoke(new Action(() =>
                                    {
                                        BackToEditBtn_Click(null, null);
                                    }));
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
                        App.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            BackToEditBtn_Click(null, null);
                        }));
                    }
                    UnitCore.Instance.PostMsgEvent_Tick.Set();
                    
                }
                else
                {
                    return;
                }
               
                

            }
            catch (Exception ex)
            {
                UnitCore.Instance.PostMsgEvent_Tick.Set();
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent(ex.Message, LogType.Both));
            }


        }

        private void RecHandle(byte[] bufBytes)
        {
            count++;
            string str = count.ToString();
            LogHelper.WriteLog("1录音准备发送！" + str);
            Dispatcher.BeginInvoke(new Action(() =>
            {                                
                try
                {
                    LogHelper.WriteLog("2录音条显示！" + str);
                    if (isRecording)
                    {
                        if (UnitCore.Instance.NetCore.IsTCPWorking)
                        {
                            Count++;
                            
                            double[] Voicedata = new double[bufBytes.Length];
                            Int16[] intVoicedata = new Int16[bufBytes.Length];
                           /* for (int i = 0; i < bufBytes.Length / 2; i++)
                            {
                                Voicedata[i] = ((double)BitConverter.ToInt16(bufBytes, i * 2)) * 10;
                                if (Voicedata[i]>32767)
                                {
                                    Voicedata[i] = 32767;
                                }
                                else if (Voicedata[i] <-32767)
                                {
                                    Voicedata[i] = -32767;
                                }
                                intVoicedata[i] = (Int16)Voicedata[i];
                                Buffer.BlockCopy(BitConverter.GetBytes(intVoicedata[i]), 0, bufBytes, i * 2, 2);
                            }

                            UnitCore.Instance.NetCore.Send((int)ModuleType.SSB, bufBytes);*/

                            /*if(Count<41)
                            {
                                Buffer.BlockCopy(bufBytes, 0, databuf, bufBytes.Length * (Count - 1), bufBytes.Length);
                            }
                            if(Count==40)
                            {
                                nAmp = calcAmp(databuf, databuf.Length);                               
                            }*/
                            Buffer.BlockCopy(bufBytes, 0, intVoicedata, 0, bufBytes.Length);
                            intVoicedata = VoiceNormalize(intVoicedata, intVoicedata.Length, nAmp);
                            Buffer.BlockCopy(intVoicedata, 0, bufBytes, 0, bufBytes.Length);
                            UnitCore.Instance.NetCore.Send((int)ModuleType.SSB, bufBytes);
                        }
                        VoiceBar.Value = UpdateVolumn(bufBytes);
                        UnitCore.Instance.MovTraceService.Save("XMTVOICE", bufBytes);
                        LogHelper.WriteLog("2录音条显示！" + str);
                    }
                                                
                    //WaveControl.Display(bufBytes);
                    
                }
                catch (Exception ex)
                {
                    UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent(ex.Message, LogType.Both));
                }
                
            }));
            LogHelper.WriteLog("3录音准备发送中！");
            
            
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
            nAmp = UnitCore.Instance.MovConfigueService.GetNormAmp();
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

            if (UnitCore.Instance.WorkMode == MonitorMode.SUBMARINE && !IsBPInitialize)
            {
                if (m_pThread_BPsend == null)
                {
                    m_pThread_BPsend = new Thread(BPSerialProcsend);//创建并挂起线程
                }
                IsBPInitialize = true;
                SetupBPDSP();
            }
            
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
                if (mode == MonitorMode.SHIP)
                {
                    cmd = "h 4 -w";
                    await UnitCore.Instance.NetCore.SendConsoleCMD(cmd);
                }
                else
                {
                    cmd = "h 1 -w";
                    await UnitCore.Instance.NetCore.SendConsoleCMD(cmd);
                }
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
                    cmd = "g "+gain;
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

        private static async void SetupBPDSP()
        {
            int m_PulseWidth = UnitCore.Instance.MovConfigueService.GetPulseWidth();
            int m_DistLimit = UnitCore.Instance.MovConfigueService.GetDistLimit();
            int m_TVCModel = UnitCore.Instance.MovConfigueService.GetTVCModel();
            int m_GainStart = UnitCore.Instance.MovConfigueService.GetGainStart();
            int m_GainUPLimit = UnitCore.Instance.MovConfigueService.GetGainUPLimit();
            int m_GainDownLimit = UnitCore.Instance.MovConfigueService.GetGainDownLimit();
            float m_BlindRegion = UnitCore.Instance.MovConfigueService.GetBlindRegion();
            int m_GateTh = UnitCore.Instance.MovConfigueService.GetGateTh();
            int m_SoundSpeed = UnitCore.Instance.MovConfigueService.GetSoundSpeed();
            int ServerMode = UnitCore.Instance.MovConfigueService.GetSERVERMODE();
            DelayTime = (int)(m_DistLimit * 2 / (float)(m_SoundSpeed) * 1000 + 140 / 400.0 * m_DistLimit * 2);
            for (int i = 0; i < 1; i++)
            {
                string cmd2 = "STPARA,";
                string str2;
                string strtvc;
                str2 = string.Format("{0},{1:D3},{2},{3:+00;-00},{4:+00;-00},{5:+00;-00},{6},{7},{8:D4}",
                    m_PulseWidth, m_DistLimit, m_TVCModel, m_GainStart, m_GainUPLimit, m_GainDownLimit, m_BlindRegion, m_GateTh, m_SoundSpeed);
                strtvc = string.Format("{0}", '*');
                byte[] Bcmd2 = new byte[str2.Length + cmd2.Length + strtvc.Length];
                str2 = cmd2 + str2 + strtvc;
                Buffer.BlockCopy(Encoding.UTF8.GetBytes(str2), 0, Bcmd2, 0, Bcmd2.Length);
                await UnitCore.Instance.CommCore.SendConsoleCMD(Bcmd2, UnitCore.Instance.MovConfigueService.GetOASID()[i].id);
                //UnitCore.Instance.PostMsgEvent_BPpara.WaitOne(3000);//等待接收完再发送下一次,配置参数时DSP从flash中读数据再写需要1s的延迟
                await TaskEx.Delay(TimeSpan.FromMilliseconds(1300));//实际测试需要等待时间大部分在1s100ms
                //Thread.Sleep(1300);
            }
            if (ServerMode == 0)//CONTRS同步连续方式需要先广播
            {
                await UnitCore.Instance.CommCore.SendConsoleCMD(Encoding.UTF8.GetBytes("CONTRS*"), 0);// SYNTRS同步轮流
                await TaskEx.Delay(TimeSpan.FromMilliseconds(200));
                //		WriteComCMD("GTRSLT*",OASSend[idcount].id,&BPSerial);//读取结果指令
                //时间校验，参数设定，启动发射
            }

            ////////////发送线程
            m_pThread_BPsend.Start();//恢复线程运行
            

        }

        void BPSerialProcsend()
        {

            while (UnitCore.Instance.CommCore.IsWorking)
            {
                if (UnitCore.Instance.MovConfigueService.GetSERVERMODE() == 1)//同步单次模式需要较多等待时间，等待回波
                {
                    UnitCore.Instance.PostMsgEvent_BPsend.WaitOne(2000);
                }
                else
                {
                    UnitCore.Instance.PostMsgEvent_BPsend.WaitOne(1000);
                }


                UnitCore.Instance.PostMsgEvent_BPsend.Reset();
                //发送消息在消息处理函数中调用发送子函数
                m_pMessage_BPsend = new Thread(new ThreadStart(OnCommBPNotifysend));
                m_pMessage_BPsend.Start();


            }
            return;
        }

        void OnCommBPNotifysend()
        {
            int PreIdCount = (idcount - 1) % (UnitCore.Instance.MovConfigueService.GetCycID().Length);
            switch (UnitCore.Instance.MovConfigueService.GetSERVERMODE())
            {
                case 0:
                    UnitCore.Instance.CommCore.SendConsoleCMD(Encoding.UTF8.GetBytes("GTRSLT*"), UnitCore.Instance.MovConfigueService.GetCycID()[0]);
                    idcount = (idcount + 1) % (UnitCore.Instance.MovConfigueService.GetCycID().Length);
                    break;
                case 1:
                    UnitCore.Instance.CommCore.SendConsoleCMD(Encoding.UTF8.GetBytes("SYNTRS*"), UnitCore.Instance.MovConfigueService.GetCycID()[0]);
                    Thread.Sleep(200);
                    UnitCore.Instance.CommCore.SendConsoleCMD(Encoding.UTF8.GetBytes("GTRSLT*"), UnitCore.Instance.MovConfigueService.GetCycID()[0]);
                    idcount = (idcount + 1) % (UnitCore.Instance.MovConfigueService.GetCycID().Length);
                    break;
                case 2:
                    UnitCore.Instance.CommCore.SendConsoleCMD(Encoding.UTF8.GetBytes("ASYTRS*"), 1);
                    Thread.Sleep(DelayTime);//经测试得等待100ms           
                    UnitCore.Instance.CommCore.SendConsoleCMD(Encoding.UTF8.GetBytes("GTRSLT*"), 1);                    
                    idcount = (idcount + 1);
                    

                    //		}
                    break;
                case 3:
                    UnitCore.Instance.CommCore.SendConsoleCMD(Encoding.UTF8.GetBytes("LAKEAS*"), UnitCore.Instance.MovConfigueService.GetCycID()[0]);
                    //		}
                    break;

                default:
                    break;
            }

            return;

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
                MainFrameViewModel.pMainFrame.MsgLog.Add(DateTime.Now.ToLongTimeString() + ":" +
                                                                         "（母船）" + SendMessageBox.Text);
            }
            else
            {

                UnitCore.Instance.MovTraceService.Save("Chart", "（潜器）" + SendMessageBox.Text);
                MainFrameViewModel.pMainFrame.MsgLog.Add(DateTime.Now.ToLongTimeString() + ":" +
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
            
            if (type == ModuleType.MFSK)
            {
                if(msg==null)
                {
                    MessageRichTextBox.ScrollToEnd();
                    return;
                }
                SetTiltle(false);
                Run run;
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
                SetTiltle(false);
                Run run;
                run = new Run("("+"跳频"+")"+msg);
                run.FontSize = 22;
                var chartmsg = new Paragraph(run);
                chartmsg.TextAlignment = TextAlignment.Right;
                chartmsg.LineHeight = 24;
                MessageDocument.Blocks.Add(chartmsg);
            }
            else if (type == ModuleType.MPSK)
            {
                SetTiltle(false);
                if (msg != null)
                {
                    Run run;
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
            int left = UnitCore.Instance.MFSK_LeftSize - SendMessageBox.Text.Length;
            LeftSize.Text = "(还可继续输入"+left+"个字)";
            if(SendMessageBox.Text.Length>0)
                SendBtn.Visibility = Visibility.Visible;
            else
                SendBtn.Visibility = Visibility.Hidden;
        }

        private void SendMessageBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
             if (e.Key== Key.Return&&SendMessageBox.Text!="")
             {
                 if (UnitCore.Instance.MFSK_LeftSize < SendMessageBox.Text.Length)
                 {
                     UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("输入的文字超出范围！", LogType.Both));
                 }
                 else
                 {
                     UnitCore.Instance.MFSK_LeftSize = UnitCore.Instance.MFSK_LeftSize - SendMessageBox.Text.Length;
                     if (UnitCore.Instance.MFSK_LeftSize <=2)
                     {
                         SendMessageBox.IsEnabled = false;
                         UnitCore.Instance.MFSK_LeftSize = 0;
                     }
                     else
                     {
                         UnitCore.Instance.MFSK_LeftSize = UnitCore.Instance.MFSK_LeftSize - 2;//如果还没满，每次都要加回车，要减去2字节
                     }

                     SendMessageBox.MaxLength = UnitCore.Instance.MFSK_LeftSize;
                     AddSendMsgToChart();
                 }
             }
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if(SendMessageBox.Text!="")
            {
                if (UnitCore.Instance.MFSK_LeftSize < SendMessageBox.Text.Length)
                {
                    UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("输入的文字超出范围！", LogType.Both));
                }
                else
                {
                    UnitCore.Instance.MFSK_LeftSize = UnitCore.Instance.MFSK_LeftSize - SendMessageBox.Text.Length;
                    if (UnitCore.Instance.MFSK_LeftSize <=2)
                    {
                        SendMessageBox.IsEnabled = false;
                        UnitCore.Instance.MFSK_LeftSize = 0;
                    }
                    else
                    {
                        UnitCore.Instance.MFSK_LeftSize = UnitCore.Instance.MFSK_LeftSize - 2;//如果还没满，每次都要加回车，要减去2字节
                    }
                    SendMessageBox.MaxLength = UnitCore.Instance.MFSK_LeftSize;
                    AddSendMsgToChart();
                }
                
            }
                
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
                    isRecording = true;
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
                        //isRecording = true;
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
         //   Recording(false);
        }
        private async void BackToEditBtn_Click(object sender, RoutedEventArgs e)
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

        private  void SendMorse(object sender, RoutedEventArgs e)
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

        private void SendMessageBox_IsEnableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (SendMessageBox.IsEnabled==true)
            {
                SendMessageBox.MaxLength = 20;
                int left = UnitCore.Instance.MFSK_LeftSize - SendMessageBox.Text.Length;
                LeftSize.Text = "(还可继续输入" + left + "个字)";
            }
           
        }

        private void ChangeXMTValue(object sender, MouseButtonEventArgs e)
        {
            if (UnitCore.Instance.NetCore.IsTCPWorking)
            {
                var cmd = "a " + slider.Value.ToString("F03");
                UnitCore.Instance.NetCore.SendConsoleCMD(cmd);
                LogHelper.WriteLog("发射幅度设置为" + slider.Value.ToString("F03"));                
            }
            var ans = UnitCore.Instance.MovConfigueService.SetXmtAmp((float)slider.Value);//保存幅度
            if (ans == false)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("保存参数出错", LogType.Both));
                return;
            }
            
        }

        private void KeyChangeXMTValue(object sender, KeyEventArgs e)
        {
            if (UnitCore.Instance.NetCore.IsTCPWorking)
            {
                var cmd = "a " + slider.Value.ToString("F03");
                UnitCore.Instance.NetCore.SendConsoleCMD(cmd);
                LogHelper.WriteLog("发射幅度设置为" + slider.Value.ToString("F03"));                
            }
            var ans = UnitCore.Instance.MovConfigueService.SetXmtAmp((float)slider.Value);//保存幅度
            if (ans == false)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("保存参数出错", LogType.Both));
                return;
            }
            
            
        }

        private int calcAmp(Int16[] Buffer,int nlength)
        {
            int[] AmpArray = new int[128];
            int NorAmp = 32767;
            //直方图统计
            for(int i=0;i<nlength;i++)
            {
                AmpArray[Math.Abs(Buffer[i])>>8]++;
            }
            //计算95%概率分布时的幅度值
            int nSum = 0;
            for(int j=0;j<128;j++)
            {
                nSum+=AmpArray[j];
                if((float)nSum/nlength>0.95)
                {
                    NorAmp = (j + 1) * 256;
                    break;
                }

            }
            return NorAmp;
        }

        private Int16[] VoiceNormalize(Int16[] Buffer, int nlength,int nAmp)
        {
            //以该幅值做归一化
            double fFactor=32767.0/nAmp;
            for(int i=0;i<nlength;i++)
            {
                if(Buffer[i]>=nAmp)
                {
                    Buffer[i]=32767;
                }
                else if(Buffer[i]<=-nAmp)
                {
                    Buffer[i]=-32767;
                }
                else
                    Buffer[i]=(Int16)(Buffer[i]*fFactor);

            }
            return Buffer;
        }





    }
}
