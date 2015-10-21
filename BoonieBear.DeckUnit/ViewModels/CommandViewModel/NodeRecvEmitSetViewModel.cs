using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using BoonieBear.DeckUnit.ACNP;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.Helps;
using MahApps.Metro.Controls.Dialogs;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;
using BoonieBear.DeckUnit.BaseType;
namespace BoonieBear.DeckUnit.ViewModels.CommandViewModel
{
    public class NodeRecvEmitSetViewModel : ViewModelBase
    {
        public override void Initialize()
        {
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            ID = new List<FilterItem>();
            for (int i = 1; i < 64; i++)
            {
                ID.Add(new FilterItem(i.ToString())); ;
            }
            NodeID = ID[0];
            RecvAutoClose = false;
            EmitAutoClose = false;
            RecvGain = 60;
            EmitGain = 60;
        }

        public override void InitializePage(object extraData)
        {

        }
        public List<FilterItem> ID
        {
            get { return GetPropertyValue(() => ID); }
            set { SetPropertyValue(() => ID, value); }
        }
        //节点号
        public FilterItem NodeID
        {
            get { return GetPropertyValue(() => NodeID); }
            set { SetPropertyValue(() => NodeID, value); }
        }

        public bool RecvAutoClose 
        {
            get { return GetPropertyValue(() => RecvAutoClose); }
            set { SetPropertyValue(() => RecvAutoClose, value); }
        }
        public bool EmitAutoClose
        {
            get { return GetPropertyValue(() => EmitAutoClose); }
            set { SetPropertyValue(() => EmitAutoClose, value); }
        }
        public int EmitGain
        {
            get { return GetPropertyValue(() => EmitGain); }
            set { SetPropertyValue(() => EmitGain, value); }
        }

        public int RecvGain
        {
            get { return GetPropertyValue(() => RecvGain); }
            set { SetPropertyValue(() => RecvGain, value); }
        }
        public bool IsProcessing
        {
            get { return GetPropertyValue(() => IsProcessing); }
            set { SetPropertyValue(() => IsProcessing, value); }
        }
        #region cmd
        public ICommand GoBackCommand
        {
            get { return GetPropertyValue(() => GoBackCommand); }
            set { SetPropertyValue(() => GoBackCommand, value); }
        }


        private void CanExecuteGoBackCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private void ExecuteGoBackCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }
        public ICommand SendCMD
        {
            get { return GetPropertyValue(() => SendCMD); }
            set { SetPropertyValue(() => SendCMD, value); }
        }


        private void CanExecuteSendCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private async void ExecuteSendCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            IsProcessing = true;
            ACNBuilder.Pack141(NodeID.num,EmitAutoClose?EmitGain:0,RecvAutoClose?RecvGain:0);
            var cmd = ACNProtocol.Package(false);
            var cl = new CommandLog();
            cl.DestID = NodeID.num;
            cl.SourceID = (int)ACNProtocol.SourceID;
            cl.LogTime = DateTime.Now;
            cl.CommID = 141;
            cl.Type = false;
            UnitCore.Instance.UnitTraceService.Save(cl, cmd);
            var result = UnitCore.Instance.NetEngine.SendCMD(cmd);
            await result;
            var ret = result.Result;
            IsProcessing = false;
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "确定";
            if (ret == false)
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "发送失败",
                UnitCore.Instance.NetEngine.Error,MessageDialogStyle.Affirmative,md);
            else
            {
                var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                dialog.Title = "节点收发设置";
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                    dialog);

                var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
                textBlock.Text = "发送成功！";

                await TaskEx.Delay(2000);

                await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, dialog);
            }
        }

        #endregion

       
    }
}
