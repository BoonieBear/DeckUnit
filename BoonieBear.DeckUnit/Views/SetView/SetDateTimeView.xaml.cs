using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using BoonieBear.DeckUnit.ACNP;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using TreeHelper = BoonieBear.DeckUnit.Helps.TreeHelper;

namespace BoonieBear.DeckUnit.Views.SetView
{
    /// <summary>
    /// SetDateTime.xaml 的交互逻辑
    /// </summary>
    public partial class SetDateTimeView : Page
    {
        private Flyout flyout1,flyout2, flyout3;
        private DispatcherTimer t;
        public SetDateTimeView()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            bool ret = false;
            Task<bool> result = null;
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "确定";
            DateTime dt = StartTime.Value;
            if (dt < DateTime.Now && TitleBox.Text == "设置休眠时间")
            {
               
                await
                    MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame,
                        "时间无效",
                       "休眠时间不能早于当前时间", MessageDialogStyle.Affirmative, md);
                
                return;
            }
            ProgressBar.Visibility = Visibility.Visible;
            switch (TitleBox.Text)
            {
                case "设置系统时间":
                    result = SetSystemTime(dt);
                    break;
                case "设置休眠时间":
                    result = SetSleepTime(dt);
                    break;
                default:
                    break;
            }
            await result;
            if(result==null)
                return;
            ret = result.Result;
            ProgressBar.Visibility = Visibility.Collapsed;
            
            if (ret == false)
            {
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "发送失败",
                UnitCore.Instance.CommEngine.Error, MessageDialogStyle.Affirmative, md);

            }
            else
            {
                var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                dialog.Title = TitleBox.Text;
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                    dialog);

                var textBlock = TreeHelper.FindChild<TextBlock>(dialog, "MessageTextBlock");
                textBlock.Text = "发送成功！";

                await TaskEx.Delay(2000);

                await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, dialog);
            }
            
        }
        private Task<bool> SetSleepTime(DateTime dt)
        {

            var cmd = MSPHexBuilder.Pack251(dt);
            return UnitCore.Instance.CommEngine.SendCMD(cmd);
        }
        private Task<bool> SetSystemTime(DateTime dt)
        {

            var cmd = MSPHexBuilder.Pack252(dt);
            return UnitCore.Instance.CommEngine.SendCMD(cmd);

        }
        private void SimplePage_Loaded(object sender, RoutedEventArgs e)
        {
            t=new DispatcherTimer(TimeSpan.FromMilliseconds(100),DispatcherPriority.Background,CheckFlyoutStatus,Dispatcher.CurrentDispatcher);
            t.Start();
            StartTime.Value = DateTime.Now;
            FormsHost.Visibility = Visibility.Visible;
            var mainwin = App.Current.MainWindow as MainFrame;
            flyout1 = mainwin.flyoutsControl.Items[0] as Flyout;
            flyout2 = mainwin.flyoutsControl.Items[1] as Flyout;
            flyout3 = mainwin.flyoutsControl.Items[2] as Flyout;
        }

        private void CheckFlyoutStatus(object sender, EventArgs e)
        {
            if (flyout1.IsOpen == false && flyout2.IsOpen == false && flyout3.IsOpen == false)
            {
                FormsHost.Visibility = Visibility.Visible;
            }
            else
            {
                FormsHost.Visibility = Visibility.Hidden;
            }
        }

        private void SimplePage_Unloaded(object sender, RoutedEventArgs e)
        {
            t.Stop();
        }

    }
}
