using System;
using System.Collections.Generic;
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
using BoonieBear.DeckUnit.Core;
using System.IO;
using TinyMetroWpfLibrary.EventAggregation;
using BoonieBear.DeckUnit.Events;

namespace BoonieBear.DeckUnit.Views
{
    /// <summary>
    /// ADPage.xaml 的交互逻辑
    /// </summary>
    public partial class ADPage : Page
    {
        public ADPage()
        {
            InitializeComponent();
            WaveControl.Initailize();
            UnitCore.Instance.Wave = WaveControl;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string path = UnitCore.Instance.UnitTraceService.AdPath;
            if (Directory.Exists(path))
                System.Diagnostics.Process.Start("explorer.exe", path);
        }

        private void TextBlock_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        }
    }
}
