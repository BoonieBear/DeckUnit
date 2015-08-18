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
using BoonieBear.DeckUnit.ACNP;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.ViewModels;
using BoonieBear.DeckUnit.ViewModels.SetViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace BoonieBear.DeckUnit.Views.SetView
{
    /// <summary>
    /// DebugView.xaml 的交互逻辑
    /// </summary>
    public partial class DebugView : Page
    {
        public DebugView()
        {
            InitializeComponent();
        }

        private async void RebootButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as DebugViewModel;
            
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "确定",
                NegativeButtonText = "取消",
                AnimateShow = true,
                AnimateHide = false
            };

            var result = await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "设置",
               "确定重启MSP430？",
               MessageDialogStyle.AffirmativeAndNegative, mySettings);



            if (result == MessageDialogResult.Affirmative)
            {
                if (vm != null)
                    vm.IsProcessing = true;
                var cmd = MSPHexBuilder.Pack249();
                await UnitCore.Instance.CommEngine.SendCMD(cmd);
                if (vm != null)
                    vm.IsProcessing = false;
            }
            
        }

        private async void ClearRebootButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as DebugViewModel;
            
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "确定",
                NegativeButtonText = "取消",
                AnimateShow = true,
                AnimateHide = false
            };

            var result = await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "设置",
               "确定要清空重启次数？",
               MessageDialogStyle.AffirmativeAndNegative, mySettings);



            if (result == MessageDialogResult.Affirmative)
            {
                if (vm != null)
                    vm.IsProcessing = true;
                var cmd = MSPHexBuilder.Pack243();
                await UnitCore.Instance.CommEngine.SendCMD(cmd);
                if (vm != null)
                    vm.IsProcessing = false;
            }
        }

        private async void DSPPowerOn_Checked(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as DebugViewModel;
            if (vm != null)
                vm.IsProcessing = true;
            var cmd = MSPHexBuilder.Pack244();
            await UnitCore.Instance.CommEngine.SendCMD(cmd);
            if (vm != null)
                vm.IsProcessing = false;
        }

        private async void DSPPowerOff_Unchecked(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as DebugViewModel;
            if (vm != null)
                vm.IsProcessing = true;
            var cmd = MSPHexBuilder.Pack245();
            await UnitCore.Instance.CommEngine.SendCMD(cmd);
            if (vm != null)
                vm.IsProcessing = false;
        }

        private async void WatchDog_Checked(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as DebugViewModel;
            if (vm != null)
                vm.IsProcessing = true;
            var cmd = MSPHexBuilder.Pack246(true);
            await UnitCore.Instance.CommEngine.SendCMD(cmd);
            if (vm != null)
                vm.IsProcessing = false;
        }

        private async void WatchDog_Unchecked(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as DebugViewModel;
            if (vm != null)
                vm.IsProcessing = true;
            var cmd = MSPHexBuilder.Pack246(false);
            await UnitCore.Instance.CommEngine.SendCMD(cmd);
            if (vm != null)
                vm.IsProcessing = false;
        }

        private async void Debug_Checked(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as DebugViewModel;
            if (vm != null)
                vm.IsProcessing = true;
            var cmd = MSPHexBuilder.Pack250(true);
            await UnitCore.Instance.CommEngine.SendCMD(cmd);
            if (vm != null)
                vm.IsProcessing = false;
        }

        private async void Debug_Unchecked(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as DebugViewModel;
            if (vm != null)
                vm.IsProcessing = true;
            var cmd = MSPHexBuilder.Pack250(false);
            await UnitCore.Instance.CommEngine.SendCMD(cmd);
            if (vm != null)
                vm.IsProcessing = false;
        }
    }
}
