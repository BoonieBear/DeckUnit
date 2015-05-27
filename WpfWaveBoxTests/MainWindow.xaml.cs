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
using BoonieBear.DeckUnit.WaveBox;
using BoonieBear.DeckUnit.VoiceManager;
namespace WpfWaveBoxTests
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private BinaryReader br;
        private byte[] _playerBuffer;
        public MainWindow()
        {
            InitializeComponent();
            FileStream fs = new FileStream("XMTvoice20120622095015.wav", FileMode.Open);
            br = new BinaryReader(fs);
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 125);
            timer.Tick += timer_Tick;
            timer.Start();
            WaveControl.PlayMode = WaveControl.Mode.Voiceplayer;
            //WaveControl.AddRecDoneHandle(saverecvoice);
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            byte[] _wave = br.ReadBytes(2048);
            if (_wave.Length != 2048)//读到头了
                br.BaseStream.Position = 0;
            if (_wave.Length != 0)
            {
                WaveControl.Display(_wave);
            }

            
        }

        private void saverecvoice(byte[] bytes)
        {
            Console.WriteLine("handle called");
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            WaveControl.Dispose();
        }
    }
}
