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
using BoonieBear.DeckUnit.Mov4500UI.Core;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using MahApps.Metro.Controls.Dialogs;
using BoonieBear.DeckUnit.Mov4500UI.ViewModel;

namespace BoonieBear.DeckUnit.Mov4500UI.Views
{
    /// <summary>
    /// LiveCaptureView.xaml 的交互逻辑
    /// </summary>
    public partial class LiveCaptureView : Page
    {
        public LiveCaptureView()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UnitCore.Instance.IsWorking)
            {
            ServiceInitial:
                bool ret = UnitCore.Instance.Start();
                if (ret == false)
                {
                    var md = new MetroDialogSettings();
                    md.AffirmativeButtonText = "重新连接";
                    md.NegativeButtonText = "修改系统设置";
                    MessageDialogResult answer = await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "启动通信机失败！",
                        UnitCore.Instance.Error, MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary, md);
                    if (answer == MessageDialogResult.Affirmative)
                    {
                        UnitCore.Instance.EventAggregator.PublishMessage(new GoSettingNavigation());
                    }
                    else
                    {
                        if (!UnitCore.Instance.Start())
                            goto ServiceInitial;
                    }
                }
                
            }
        }
    }
}
