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
        private Stream Updatefile;
        public GlobalSettingView()
        {
            InitializeComponent();
             
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenMspFile = new OpenFileDialog();
            if (OpenMspFile.ShowDialog() == true)
            {
                try
                {
                    if (UnitCore.Instance.NetCore.IsWorking)
                    {
                        t = new DispatcherTimer(TimeSpan.FromMilliseconds(100),DispatcherPriority.Background,RefreshPercentage,Dispatcher.CurrentDispatcher);
                        t.Start();
                        Updatefile = OpenMspFile.OpenFile();
                        PercentageRing.Visibility = Visibility.Visible;
                        var ret = await UnitCore.Instance.NetCore.SendFile(Updatefile);
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
                    PercentageRing.Visibility = Visibility.Collapsed;
                    if(t!=null)
                        t.Stop();

                }
                catch (Exception MyEx)
                {
                    if (Updatefile != null)
                        Updatefile.Close();
                    PercentageRing.Visibility = Visibility.Collapsed;
                    if (t != null)
                        t.Stop();
                    UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent(MyEx.Message, LogType.Both));
                }
            }
        }

        private void RefreshPercentage(object sender, EventArgs e)
        {
            PercentageRing.Percentange = UnitCore.Instance.NetCore.SendBytes * 100 / (int)Updatefile.Length;
        }

    }
}
