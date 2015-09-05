using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using TinyMetroWpfLibrary.Utility;
using BoonieBear.DeckUnit.ViewModels.SetViewModel;
namespace BoonieBear.DeckUnit.Views.SetView
{
    /// <summary>
    /// PingTestView.xaml 的交互逻辑
    /// </summary>
    public partial class PingTestView : Page
    {
        private string mintext = "1234567890";

        private string mediumtext = "1234567890abcdefghijklmnopqrstuvwxyz" +
                                    "1234567890abcdefghijklmnopqrstuvwxyz" +
                                    "1234567890abcdefghijklmnopqrstuvwxyz" +
                                    "1234567890abcdefghijklmnopqrstuvwxyz" +
                                    "1234567890abcdefghijklmnopqrstuvwxyz" +
                                    "1234567890abcdefghijklmnopqrstuvwxyz" +
                                    "1234567890abcdefghijklmnopqrstuvwxyz";

        private string maxtext = "回环测试1234567890abcdefghijklmnopqrstuvwxyz" +
                                 "回环测试1234567890abcdefghijklmnopqrstuvwxyz" +
                                 "回环测试1234567890abcdefghijklmnopqrstuvwxyz" +
                                 "回环测试1234567890abcdefghijklmnopqrstuvwxyz" +
                                 "回环测试1234567890abcdefghijklmnopqrstuvwxyz" +
                                 "回环测试1234567890abcdefghijklmnopqrstuvwxyz" +
                                 "回环测试1234567890abcdefghijklmnopqrstuvwxyz" +
                                 "回环测试1234567890abcdefghijklmnopqrstuvwxyz" +
                                 "回环测试1234567890abcdefghijklmnopqrstuvwxyz" +
                                 "回环测试1234567890abcdefghijklmnopqrstuvwxyz" +
                                 "回环测试回环测试回环测试回环测试回环测试";
        public PingTestView()
        {
            InitializeComponent();
            ModeBox.SelectedIndex = 0;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CMDTextBox==null)
                return;
            PingTestViewModel pm = (PingTestViewModel) this.DataContext;
            CMDTextBox.IsReadOnly = true;
            
            switch (ModeBox.SelectedIndex)
            {
                case 0:
                    pm.PingCMD= mintext;
                    break;
                case 1:
                    pm.PingCMD = mediumtext;
                    break;
                case 2:
                    pm.PingCMD = maxtext;
                    break;
                case 3:
                    pm.PingCMD = string.Empty;
                    CMDTextBox.IsReadOnly = false;
                    break;
                default:
                    break;

            }
        }

        private void CMDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var str = CMDTextBox.Text;
                var hex = StringHexConverter.ConvertStrToHex(str);
                var pkg = StringHexConverter.ConvertHexToChar(hex);
                while (pkg.Length > 480)
                {
                    str = str.Substring(0, str.Length - 1);
                   hex = StringHexConverter.ConvertStrToHex(str);
                   pkg = StringHexConverter.ConvertHexToChar(hex);
                }
                CMDTextBox.Text = str;
                if (pkg.Length>0&&sendlength!=null)
                    sendlength.Text = "待发送" + pkg.Length+"字节";
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

        private void RecvTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var str = RecvTextBox.Text;
                var hex = StringHexConverter.ConvertStrToHex(str);
                var pkg = StringHexConverter.ConvertHexToChar(hex);
                if (pkg.Length > 0)
                {
                    recvlength.Text = "收到" + pkg.Length+"字节";
                    if (RecvTextBox.Text.Equals(CMDTextBox.Text))
                        recvlength.Text += " 回传正确！";
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
            
        }
    }
}
