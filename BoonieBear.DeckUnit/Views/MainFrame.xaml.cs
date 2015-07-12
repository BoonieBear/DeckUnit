using System;
using System.Threading;
using System.Threading.Tasks;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.Events;
using TinyMetroWpfLibrary.Controller;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using BoonieBear.DeckUnit.ViewModels;
namespace BoonieBear.DeckUnit.Views
{

    public partial class MainFrame
    {
        public MainFrame()
        {
            InitializeComponent();
            MainFrameViewModel.pMainFrame.DialogCoordinator = DialogCoordinator.Instance;
            //DataContext = MainFrameViewModel.pMainFrame;
            
            Kernel.Instance.Controller.SetRootFrame(ContentFrame);
        }

        private void ContentFrame_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

            ProgressDialogController remote = null;
            UnitCore.Instance.Start();
            /*
            var remoteTask = this.ShowProgressAsync("请稍候...", "正在初始化系统");
            Task.Factory.StartNew(() => Thread.Sleep(2000)).ContinueWith(x => Dispatcher.Invoke(new Action(() =>
            {
                remote = remoteTask.Result;
                
            }))).ContinueWith(obj =>
            {
                //UnitCore.Instance.Start();
                remote.SetIndeterminate();
                //remote.SetCancelable(true);
                Dispatcher.Invoke(new Action(() =>
                {
                    if (UnitCore.Instance.ServiceOK)
                    {
                        remote.SetMessage("初始化成功!");
                    }
                    else
                    {
                        remote.SetMessage("初始化失败,详细错误信息请查看系统日志");
                    }
                }));
                Thread.Sleep(3000);
                Dispatcher.Invoke(new Action(() => remote.CloseAsync().ContinueWith(x =>
                {
                    if (!UnitCore.Instance.ServiceOK)
                    {
                        //导航到Home界面，下面的是示例
                        UnitCore.Instance.EventAggregator.PublishMessage(new GoHomePageNavigationEvent());
                        
                    }
                })));
            });*/

        }

        private void FlyOutView(int index)
        {
            var flyout = this.flyoutsControl.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }

            flyout.IsOpen = !flyout.IsOpen;
        }
        private void LaunchDebugView(object sender, System.Windows.RoutedEventArgs e)
        {
            FlyOutView(0);
        }

        private void LaunchDataView(object sender, System.Windows.RoutedEventArgs e)
        {
            FlyOutView(1);
        }
    }
}
