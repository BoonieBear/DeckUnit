using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace WpfWaveBoxTests
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private BinaryReader br;
        public MainWindow()
        {
            InitializeComponent();
            FileStream fs = new FileStream("Ch1AD20121210102223.dat", FileMode.Open);
            br = new BinaryReader(fs);
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            timer.Tick += timer_Tick;
            timer.Start();
            WaveControl.isPlaying = true;
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            byte[] _wave = br.ReadBytes(4096);
            if (_wave.Length != 4096)//读到头了
                br.BaseStream.Position = 0;
            if (_wave.Length != 0)
            {
                WaveControl.Display(_wave);
            }


        }
    }
}
