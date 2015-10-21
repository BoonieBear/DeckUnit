using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using BoonieBear.DeckUnit.ACNP;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.Helps;
using MahApps.Metro.Controls.Dialogs;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;
using System.Collections.ObjectModel; 
namespace BoonieBear.DeckUnit.ViewModels.SetViewModel
{
    public class RefreshNodeConfigViewModel : ViewModelBase
    {
        public override void Initialize()
        {
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            SendCMD = RegisterCommand(ExecuteSendCMD, CanExecuteSendCMD, true);
            var addr = Enum.GetValues(typeof (DeviceAddr));
            DeviceInfo = new List<string>();
            foreach (var a in addr)
            {
                DeviceInfo.Add(a.ToString());
            }
            Comm2Device = 0;
            Comm3Device = 0;
            var Type = Enum.GetValues(typeof (ACNP.EmitType));
            EmitType= new List<string>();
            foreach (var a in Type)
            {
                EmitType.Add(a.ToString());
            }
            IsProcessing = false;
            EmitIndex = 0;
            NodeType = 0;
            AccessMode = 0;
            NetSwitch = 0;
            BuoyID = 0;
            ID = 0;
            Emit = 1;
            Long = "118.2345";
            Lat = "29.123";
        }

        public override void InitializePage(object extraData)
        {
            
        }
        public bool IsProcessing
        {
            get { return GetPropertyValue(() => IsProcessing); }
            set { SetPropertyValue(() => IsProcessing, value); }
        }
        public uint BuoyID
        {
            get { return GetPropertyValue(() => BuoyID); }
            set { SetPropertyValue(() => BuoyID, value); }
        }
        public int ID
        {
            get { return GetPropertyValue(() => ID); }
            set { SetPropertyValue(() => ID, value); }
        }
        public int NodeType
        {
            get { return GetPropertyValue(() => NodeType); }
            set
            {
                if (value == 0)
                {
                    AccessMode = 0;
                }
                SetPropertyValue(() => NodeType, value);
            }
        }
        public int AccessMode
        {
            get { return GetPropertyValue(() => AccessMode); }
            set { SetPropertyValue(() => AccessMode, value); }
        }
        public List<String> DeviceInfo
        {
            get { return GetPropertyValue(() => DeviceInfo); }
            set { SetPropertyValue(() => DeviceInfo, value); }
        }

        public List<String> EmitType
        {
            get { return GetPropertyValue(() => EmitType); }
            set { SetPropertyValue(() => EmitType, value); }
        }

        public int Emit
        {
            get { return GetPropertyValue(() => Emit); }
            set { SetPropertyValue(() => Emit, value); }
        }
        public int Comm2Device
        {
            get { return GetPropertyValue(() => Comm2Device); }
            set { SetPropertyValue(() => Comm2Device, value); }
        }
        public int Comm3Device
        {
            get { return GetPropertyValue(() => Comm3Device); }
            set { SetPropertyValue(() => Comm3Device, value); }
        }

        public int NetSwitch
        {
            get { return GetPropertyValue(() => NetSwitch); }
            set { SetPropertyValue(() => NetSwitch, value); }
        }
        public int EmitIndex
        {
            get { return GetPropertyValue(() => EmitIndex); }
            set { SetPropertyValue(() => EmitIndex, value); }
        }
        public string Long
        {
            get { return GetPropertyValue(() => Long); }
            set { SetPropertyValue(() => Long, value); }
        }
        public string Lat
        {
            get { return GetPropertyValue(() => Lat); }
            set { SetPropertyValue(() => Lat, value); }
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
            eventArgs.CanExecute = !IsProcessing;
        }


        private async void ExecuteSendCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            bool ret = false;
            IsProcessing = true;
            Task<bool> result = null;
            var mc = new ModermConfig();
            mc.Buoyid = BuoyID;
            mc.Com2Device = Convert.ToUInt32(Enum.Parse(typeof(DeviceAddr), DeviceInfo[Comm2Device]));
            mc.Com3Device = Convert.ToUInt32(Enum.Parse(typeof(DeviceAddr), DeviceInfo[Comm3Device]));
            mc.Emitnum = (uint)Emit;
            mc.Emittype = Convert.ToUInt32(Enum.Parse(typeof(EmitType), EmitType[EmitIndex]));
            mc.Lang = double.Parse(Long);
            mc.Lati = double.Parse(Lat);
            mc.Netswitch = (uint) NetSwitch;
            mc.Nodeid = (uint) ID;
            mc.Nodetype = (uint) NodeType;
            mc.Accessmode = (uint) AccessMode;
            var cmd = MSPHexBuilder.Pack253(mc);
            result = UnitCore.Instance.CommEngine.SendCMD(cmd);
            await result;
            ret = result.Result;
            IsProcessing = false;
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "确定";
            if (ret == false)
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "发送失败",
                UnitCore.Instance.CommEngine.Error,MessageDialogStyle.Affirmative,md);
            else
            {
                var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                dialog.Title = "节点配置";
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
