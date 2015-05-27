using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using TinyMetroWpfLibrary.Controller;
using MahApps.Metro.Controls.Dialogs;
using BoonieBear.DeckUnit.Mov4500UI.Core;
namespace BoonieBear.DeckUnit.Mov4500UI.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainFrame
    {
        public MainFrame()
        {
            InitializeComponent();
            Kernel.Instance.Controller.SetRootFrame(ContentFrame);
        }

        private void ContentFrame_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Splasher.CloseSplash();
            Kernel.Instance.Controller.NavigateToPage("Views/HomePageView.xaml");

        }
    }
}
