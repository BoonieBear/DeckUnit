using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using BoonieBear.DeckUnit.ACMP;
using BoonieBear.DeckUnit.ICore;
using BoonieBear.DeckUnit.Mov4500UI.Core;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using DevExpress.Xpf.Core;
using Microsoft.Win32;

namespace BoonieBear.DeckUnit.Mov4500UI.Views
{
    /// <summary>
    /// GlobalSettingView.xaml 的交互逻辑
    /// </summary>
    public partial class GlobalSettingView : Page
    {
        private DispatcherTimer t;
        private DispatcherTimer modet;
        private Stream Updatefile;
        public GlobalSettingView()
        {
            InitializeComponent();
            modet = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Background, RefreshModeBox, Dispatcher.CurrentDispatcher);
            modet.Start();
        }

        private void RefreshModeBox(object sender, EventArgs e)
        {
            if (UnitCore.Instance.NetCore.IsTCPWorking)
                ModeBox.IsEnabled = false;
            else
                ModeBox.IsEnabled = true;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenMspFile = new OpenFileDialog();
            if (OpenMspFile.ShowDialog() == true)
            {
                try
                {
                    if (UnitCore.Instance.NetCore.IsTCPWorking)
                    {
                        t = new DispatcherTimer(TimeSpan.FromMilliseconds(100),DispatcherPriority.Background,RefreshPercentage,Dispatcher.CurrentDispatcher);
                        t.Start();
                        Updatefile = OpenMspFile.OpenFile();
                        ConfigViewer.Opacity = 0.2;
                        PercentageRing.IsOpen = true;
                        var ret = await UnitCore.Instance.NetCore.DownloadFile(Updatefile,DownLoadFileType.FixFirm);
                        if (ret)
                        {
                            UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("下载数据完成", LogType.OnlyInfo));
                        }

                        else
                        {
                            UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("下载数据失败，请检查通信机网络", LogType.OnlyInfo));
                        }
                            

                    }
                    else
                    {

                        UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("请连接通信机", LogType.OnlyInfo));
                    }
                    if (Updatefile!=null)
                        Updatefile.Close();
                    PercentageRing.IsOpen = false;
                    ConfigViewer.Opacity = 1;
                    if(t!=null)
                        t.Stop();

                }
                catch (Exception MyEx)
                {
                    if (Updatefile != null)
                        Updatefile.Close();
                    PercentageRing.IsOpen = false;
                    ConfigViewer.Opacity = 1;
                    if (t != null)
                        t.Stop();
                    UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(MyEx, LogType.Both));
                }
            }
        }

        private void RefreshPercentage(object sender, EventArgs e)
        {
            try
            {
                if (Updatefile.CanRead)//防止烧写完关闭文件时，定是器还未关闭
                {
                    PercentageRing.Percentange = UnitCore.Instance.NetCore.SendBytes * 100 / (int)Updatefile.Length;
                }
            }
            catch (Exception MyEx)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(MyEx, LogType.OnlyLog));
            }
            
            
        }

        private async void ButtonReset_Click(object sender, RoutedEventArgs e)//重启机芯
        {
            if (UnitCore.Instance.NetCore.IsTCPWorking)
            {
                await TaskEx.Delay(TimeSpan.FromMilliseconds(300));//留出时间给DSP读取程序
                var ret = await UnitCore.Instance.NetCore.ResetMechan();
                if (ret)
                {
                    UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("机芯重启完成", LogType.OnlyInfo));
                    App.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        UnitCore.Instance.NetCore.StopTCpService();
                        UWVConnect.IsChecked = false;
                        ShipConnect.IsChecked = false;
                    }));
                }

                else
                {
                    UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("机芯重启失败，请检查通信机网络", LogType.OnlyInfo));
                }
            }
            else 
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("请连接通信机", LogType.OnlyInfo));

            }
            
            

            
        }

    }
}
