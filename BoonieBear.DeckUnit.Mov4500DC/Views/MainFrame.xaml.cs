using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using BoonieBear.DeckUnit.Mov4500UI.Helpers;
using TinyMetroWpfLibrary.Controller;
using MahApps.Metro.Controls.Dialogs;
using BoonieBear.DeckUnit.Mov4500UI.Core;
using BoonieBear.DeckUnit.Mov4500UI.ViewModel;
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
            MainFrameViewModel.pMainFrame.DialogCoordinator = DialogCoordinator.Instance;
            Kernel.Instance.Controller.SetRootFrame(ContentFrame);

        }

        private  void ContentFrame_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            
            Application.Current.MainWindow = this;
            Kernel.Instance.Controller.NavigateToPage("Views/HomePageView.xaml");
            Splasher.CloseSplash();
        }

        private void MetroWindow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }
    }
}
