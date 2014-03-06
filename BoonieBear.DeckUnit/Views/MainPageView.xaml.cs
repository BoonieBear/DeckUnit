using System.Windows;
using System.Windows.Controls;
using BoonieBear.DeckUnit.ViewModels;

namespace BoonieBear.DeckUnit.Views
{

    public partial class MainPageView : Page
    {
        public MainPageView()
        {
            InitializeComponent();
        }

        private void MainformPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            MainFrameViewModel.pMainFrame.IsShowBottomBar = Visibility.Hidden;
            MainFrameViewModel.pMainFrame.IsShowTopBar = Visibility.Hidden;
        }


    }
}
