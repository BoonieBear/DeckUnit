using System;
using System.Threading.Tasks;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.Events;
using BoonieBear.TinyMetro.WPF.Controller;
using MahApps.Metro.Controls.Dialogs;

namespace BoonieBear.DeckUnit.Views
{

    public partial class MainFrame
    {
        public MainFrame()
        {
            InitializeComponent();
            Kernel.Instance.Controller.SetRootFrame(ContentFrame);
        }

        private void ContentFrame_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            UnitCore.Instance = new UnitCore();
            ProgressDialogController remote = null;
            var remoteTask = this.ShowProgressAsync("请稍候...", "正在初始化系统");
            Task.Factory.StartNew(() => UnitCore.Instance.Init()).ContinueWith(x => Dispatcher.Invoke(new Action(() =>
            {
                remote = remoteTask.Result;
            }))).ContinueWith(obj =>
            {
                remote.SetIndeterminate();
                //remote.SetCancelable(true);
                Dispatcher.Invoke(new Action(() =>
                {
                    if (!UnitCore.Instance.Initailed)
                    {
                        remote.SetMessage("初始化失败,详细错误信息请查看系统日志");
                    }
                    else
                    {
                        remote.SetMessage("初始化成功!您现在可以使用甲板单元的所有功能");
                        
                    }
                }));
                System.Threading.Thread.Sleep(3000);
                Dispatcher.Invoke(new Action(() => remote.CloseAsync().ContinueWith(x =>
                {
                    if (!UnitCore.Instance.Initailed)
                    {
                        //导航到设置界面，下面的是示例
                        //UnitCore.Instance.EventAggregator.PublishMessage(new GoSystemResourceNavigationRequest());
                    }
                })));
            });
        }
    }
}
