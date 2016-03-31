using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using BoonieBear.DeckUnit.ACNP;
using BoonieBear.DeckUnit.BaseType;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.Helps;
using BoonieBear.DeckUnit.Models;
using MahApps.Metro.Controls.Dialogs;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;
using System.Collections.ObjectModel;

namespace BoonieBear.DeckUnit.ViewModels.CommandViewModel
{
    
    public class SetNodeInfoViewModel : ViewModelBase
    {
        public override void Initialize()
        {
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            SendCMD = RegisterCommand(ExecuteSendCMD, CanExecuteSendCMD, true);
            AddNodeToList = RegisterCommand(ExecuteAddNodeToList, CanExecuteAddNodeToList, true);
            DelNodeFromList = RegisterCommand(ExecuteDelNodeFromList, CanExecuteDelNodeFromList, true);
            SaveNodeList = RegisterCommand(ExecuteSaveNodeList, CanExecuteSaveNodeList, true);
            ID = new List<FilterItem>();
            NodeList = new ObservableCollection<Item>();
            for (int i = 1; i < 64; i++)
            {
                ID.Add(new FilterItem(i.ToString()));
            }
            DeviceInfo = new List<string>();
            var addr = Enum.GetValues(typeof(DeviceAddr));
            foreach (var a in addr)
            {
                DeviceInfo.Add(a.ToString());
            }
            NodeID = ID[0];
            AddID = NodeID;
            IsProcessing = false;
            ChoseSet1 = true;
            ChoseSet2 = false;
            ChoseSet3 = false;
            ChoseSet4 = false;
            ChoseSet5 = false;
            ChoseSet6 = false;
            ChoseSet7 = false;
            ChoseSet8 = false;
            ChoseSet9 = false;
            ChoseSet10 = false;
            ChoseSet11 = false;
            ChoseSet12 = false;
            ChoseSet13 = false;
            ChoseSet14 = false;
            ChoseSet15 = false;
            ChoseSet16 = false;
            Lng = 0.0f;
            Lat = 0.0f;
            NodeType = 0;
            Emit = 1;
            Depth = 50;
            Energy = 50;
            Comm2Device = 0;
            Comm3Device = 0;
        }

        public override void InitializePage(object extraData)
        {
            NodeList.Clear();
            if (UnitCore.Instance.NodeInfoMap.Count > 0)
            {
                foreach (var key in UnitCore.Instance.NodeInfoMap.Keys)
                {
                    var name = (string) key;
                    var litename = name.TrimStart('节', '点');
                    NodeList.Add(new Item(int.Parse(litename), name));
                }
                
                CurrentItem = NodeList.ElementAt(0);
            }
            else
            {
                CurrentItem = null;
            }
        }
        public List<FilterItem> ID
        {
            get { return GetPropertyValue(() => ID); }
            set { SetPropertyValue(() => ID, value); }
        }
        public List<String> DeviceInfo
        {
            get { return GetPropertyValue(() => DeviceInfo); }
            set { SetPropertyValue(() => DeviceInfo, value); }
        }
        //节点号
        public FilterItem NodeID
        {
            get { return GetPropertyValue(() => NodeID); }
            set { SetPropertyValue(() => NodeID, value); }
        }
        public int AddIDIndex
        {
            get { return GetPropertyValue(() => AddIDIndex); }
            set { SetPropertyValue(() => AddIDIndex, value); }
        }
        public FilterItem AddID
        {
            get { return GetPropertyValue(() => AddID); }
            set { SetPropertyValue(() => AddID, value); }
        }
        public Item CurrentItem
        {
            get { return GetPropertyValue(() => CurrentItem); }
            set
            {
                SetPropertyValue(() => CurrentItem, value);
                UpdateInfoRect(value);
            }
        }
        public ObservableCollection<Item> NodeList
        {
            get { return GetPropertyValue(() => NodeList); }
            set { SetPropertyValue(() => NodeList, value); }
        }
        private void UpdateInfoRect(Item value)
        {
            if(value==null)
                return;
            var ba = (BitArray) UnitCore.Instance.NodeInfoMap[CurrentItem.Name];
            ACNProtocol.GetDataForParse(ba);
            int nodeid = ACNProtocol.GetIntValueFromBit(6);
            int nodetype = ACNProtocol.GetIntValueFromBit(1);
            int emit = ACNProtocol.GetIntValueFromBit(3);
            int set1 = ACNProtocol.GetIntValueFromBit(8);
            int set2 = ACNProtocol.GetIntValueFromBit(8);
            int energy = ACNProtocol.GetIntValueFromBit(3);

            Int16 commtype = (short)ACNProtocol.GetIntValueFromBit(16);
            byte[] b = BitConverter.GetBytes(commtype);
            BitArray a = new BitArray(b);
            int n = ACNProtocol.GetIntValueFromBit(28);
            double lang = 0;
            if (n >> 27 == 1)//西经
            {
                n &= 0x7ffffff;
                lang = (double)n / 10000 / 60;
                lang = -lang;
            }
            else//北纬
            {
                n &= 0x7ffffff;
                lang = (double)n / 10000 / 60;

            }
            n = ACNProtocol.GetIntValueFromBit(28);
            double lat = 0;
            if (n >> 27 == 1)//南纬
            {
                n &= 0x7ffffff;
                lat = (double)n / 10000 / 60;
                lat = -lang;
            }
            else//北纬
            {
                n &= 0x7ffffff;
                lat = (double)n / 10000 / 60;

            }
            double depth = ACNProtocol.GetIntValueFromBit(14) * 0.5;
            ACNProtocol.Clear();
            AddIDIndex = nodeid - 1;
            NodeType = nodetype;
            Emit = emit;
            Comm2DeviceName = Enum.GetName(typeof (DeviceAddr), set1);
            Comm3DeviceName = Enum.GetName(typeof(DeviceAddr), set2);
            if (energy == 0) Energy = 5;
            else if (energy == 1) Energy = 20;
            else if (energy == 2) Energy = 35;
            else if (energy == 3) Energy = 50;
            else if (energy == 4) Energy = 65;
            else if (energy == 5) Energy = 80;
            else if (energy == 6) Energy = 95;
            
            Lng = lang;
            Lat = lat;
            Depth = depth;
            ChoseSet1 = a[0];
            ChoseSet2 = a[1];
            ChoseSet3 = a[2];
            ChoseSet4 = a[3];
            ChoseSet5 = a[4];
            ChoseSet6 = a[5];
            ChoseSet7 = a[6];
            ChoseSet8 = a[7];
            ChoseSet9 = a[8];
            ChoseSet10 = a[9];
            ChoseSet11 = a[10];
            ChoseSet12 = a[11];
            ChoseSet13 = a[12];
            ChoseSet14 = a[13];
            ChoseSet15 = a[14];
            ChoseSet16 = a[15];
        }

        private  BitArray PackageInfo()
        {
            int[] dat = new int[1];
            ACNProtocol.Clear();
            ACNProtocol.InitForPack(115);
            dat[0] = AddID.num;
            ACNProtocol.OutPutIntBit(dat, 6);
            dat[0] = (NodeType)==0 ? 0 : 1;
            ACNProtocol.OutPutIntBit(dat, 1);
            dat[0] = Emit;
            ACNProtocol.OutPutIntBit(dat, 3);
            dat[0] = Convert.ToInt32(Enum.Parse(typeof(DeviceAddr), Comm2DeviceName));
            ACNProtocol.OutPutIntBit(dat, 8);
            dat[0] = Convert.ToInt32(Enum.Parse(typeof(DeviceAddr), Comm3DeviceName));
            ACNProtocol.OutPutIntBit(dat, 8);
            int energy = Energy;
            if (energy < 5) dat[0] = 0;
            else if ((energy >= 5) && (energy < 20)) dat[0] = 1;
            else if ((energy >= 20) && (energy < 35)) dat[0] = 2;
            else if ((energy >= 35) && (energy < 50)) dat[0] = 3;
            else if ((energy >= 50) && (energy < 65)) dat[0] = 4;
            else if ((energy >= 65) && (energy < 80)) dat[0] = 5;
            else if ((energy >= 80) && (energy < 95)) dat[0] = 6;
            else if ((energy >= 95)) dat[0] = 7;
            ACNProtocol.OutPutIntBit(dat, 3);
            BitArray a = new BitArray(16);
            a[0] = ChoseSet1;
            a[1] = ChoseSet2;
            a[2] = ChoseSet3;
            a[3] = ChoseSet4;
            a[4] = ChoseSet5;
            a[5] = ChoseSet6;
            a[6] = ChoseSet7;
            a[7] = ChoseSet8;
            a[8] = ChoseSet9;
            a[9] = ChoseSet10;
            a[10] = ChoseSet11;
            a[11] = ChoseSet12;
            a[12] = ChoseSet13;
            a[13] = ChoseSet14;
            a[14] = ChoseSet15;
            a[15] = ChoseSet16;
            a.CopyTo(dat, 0);
            if (dat[0] == 0) //no true
            {
                ACNProtocol.Clear();
                var md = new MetroDialogSettings();
                md.AffirmativeButtonText = "确定";
                MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "至少选择一项通信制式",
                    UnitCore.Instance.NetEngine.Error, MessageDialogStyle.Affirmative, md);
                return null;
            }
            ACNProtocol.OutPutArrayBit(a);


            if (Lng < 0 && Lng>=-180)
            {
                dat[0] = 0x8ffffff + (int)((double)Math.Abs(Lng) * 60 * 10000);
            }
            else if (Lng >= 0 && Lng<=180)
            {
                dat[0] = (int)((double)Math.Abs(Lng) * 60 * 10000);
            }
            else//wrong input
            {
                //
                ACNProtocol.Clear();
                var md = new MetroDialogSettings();
                md.AffirmativeButtonText = "确定";
                MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "经度不在有效范围内（-180~180）",
                    UnitCore.Instance.NetEngine.Error, MessageDialogStyle.Affirmative, md);
                return null;
            }
            ACNProtocol.OutPutIntBit(dat, 28);
            if (Lat <= 0&&Lat>=-90)
            {
                dat[0] = 0x8ffffff + (int)((double)Math.Abs(Lat) * 60 * 10000);
            }
            else if(Lat>0&&Lat<=90)
            {
                dat[0] = (int)((double)Math.Abs(Lat) * 60 * 10000);
            }
            else//wrong input
            {
                //
                ACNProtocol.Clear();
                var md = new MetroDialogSettings();
                md.AffirmativeButtonText = "确定";
                MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "纬度不在有效范围内（-90~90）",
                    UnitCore.Instance.NetEngine.Error, MessageDialogStyle.Affirmative, md);
                return null;
            }
            ACNProtocol.OutPutIntBit(dat, 28);
            dat[0] = (int)(Depth / 0.5);
            ACNProtocol.OutPutIntBit(dat, 14);
            return ACNProtocol.packdata;
            
        }

        public bool IsProcessing
        {
            get { return GetPropertyValue(() => IsProcessing); }
            set { SetPropertyValue(() => IsProcessing, value); }
        }

        public double Lng
        {
            get { return GetPropertyValue(() => Lng); }
            set { SetPropertyValue(() => Lng, value); }
        }
        public double Lat
        {
            get { return GetPropertyValue(() => Lat); }
            set { SetPropertyValue(() => Lat, value); }
        }
        public int NodeType
        {
            get { return GetPropertyValue(() => NodeType); }
            set { SetPropertyValue(() => NodeType, value);}
        }
        public int Emit
        {
            get { return GetPropertyValue(() => Emit); }
            set { SetPropertyValue(() => Emit, value); }
        }
        public double Depth
        {
            get { return GetPropertyValue(() => Depth); }
            set { SetPropertyValue(() => Depth, value); }
        }
        public int Energy
        {
            get { return GetPropertyValue(() => Energy); }
            set { SetPropertyValue(() => Energy, value); }
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
        public string Comm2DeviceName
        {
            get { return GetPropertyValue(() => Comm2DeviceName); }
            set { SetPropertyValue(() => Comm2DeviceName, value); }
        }
        public string Comm3DeviceName
        {
            get { return GetPropertyValue(() => Comm3DeviceName); }
            set { SetPropertyValue(() => Comm3DeviceName, value); }
        }
        public bool ChoseSet1
        {
            get { return GetPropertyValue(() => ChoseSet1); }
            set { SetPropertyValue(() => ChoseSet1, value); }
        }
        public bool ChoseSet2
        {
            get { return GetPropertyValue(() => ChoseSet2); }
            set { SetPropertyValue(() => ChoseSet2, value); }
        }
        public bool ChoseSet3
        {
            get { return GetPropertyValue(() => ChoseSet3); }
            set { SetPropertyValue(() => ChoseSet3, value); }
        }
        public bool ChoseSet4
        {
            get { return GetPropertyValue(() => ChoseSet4); }
            set { SetPropertyValue(() => ChoseSet4, value); }
        }
        public bool ChoseSet5
        {
            get { return GetPropertyValue(() => ChoseSet5); }
            set { SetPropertyValue(() => ChoseSet5, value); }
        }
        public bool ChoseSet6
        {
            get { return GetPropertyValue(() => ChoseSet6); }
            set { SetPropertyValue(() => ChoseSet6, value); }
        }
        public bool ChoseSet7
        {
            get { return GetPropertyValue(() => ChoseSet7); }
            set { SetPropertyValue(() => ChoseSet7, value); }
        }
        public bool ChoseSet8
        {
            get { return GetPropertyValue(() => ChoseSet8); }
            set { SetPropertyValue(() => ChoseSet8, value); }
        }
        public bool ChoseSet9
        {
            get { return GetPropertyValue(() => ChoseSet9); }
            set { SetPropertyValue(() => ChoseSet9, value); }
        }
        public bool ChoseSet10
        {
            get { return GetPropertyValue(() => ChoseSet10); }
            set { SetPropertyValue(() => ChoseSet10, value); }
        }
        public bool ChoseSet11
        {
            get { return GetPropertyValue(() => ChoseSet11); }
            set { SetPropertyValue(() => ChoseSet11, value); }
        }
        public bool ChoseSet12
        {
            get { return GetPropertyValue(() => ChoseSet12); }
            set { SetPropertyValue(() => ChoseSet12, value); }
        }
        public bool ChoseSet13
        {
            get { return GetPropertyValue(() => ChoseSet13); }
            set { SetPropertyValue(() => ChoseSet13, value); }
        }
        public bool ChoseSet14
        {
            get { return GetPropertyValue(() => ChoseSet14); }
            set { SetPropertyValue(() => ChoseSet14, value); }
        }
        public bool ChoseSet15
        {
            get { return GetPropertyValue(() => ChoseSet15); }
            set { SetPropertyValue(() => ChoseSet15, value); }
        }
        public bool ChoseSet16
        {
            get { return GetPropertyValue(() => ChoseSet16); }
            set { SetPropertyValue(() => ChoseSet16, value); }
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

        public ICommand AddNodeToList
        {
            get { return GetPropertyValue(() => AddNodeToList); }
            set { SetPropertyValue(() => AddNodeToList, value); }
        }


        private void CanExecuteAddNodeToList(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private void ExecuteAddNodeToList(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            string id= AddID.Text.PadLeft(2, '0');
            var b = PackageInfo();
            if (b == null)
                return;
            if (UnitCore.Instance.NodeInfoMap.ContainsKey("节点" + id))//已经有了
            { 
                UnitCore.Instance.NodeInfoMap.Remove("节点" + id);
            }
            else
            {
                NodeList.Add(new Item(int.Parse(id), "节点"+id));
                
            }
            UnitCore.Instance.NodeInfoMap.Add("节点" + id, b);
            
        }
        public ICommand DelNodeFromList
        {
            get { return GetPropertyValue(() => DelNodeFromList); }
            set { SetPropertyValue(() => DelNodeFromList, value); }
        }


        private void CanExecuteDelNodeFromList(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private void ExecuteDelNodeFromList(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if(CurrentItem==null)
                return;
            else
            {
                var name = CurrentItem.Name;
                NodeList.Remove(CurrentItem);
                UnitCore.Instance.NodeInfoMap.Remove(name);
            }
        }
        public ICommand SaveNodeList
        {
            get { return GetPropertyValue(() => SaveNodeList); }
            set { SetPropertyValue(() => SaveNodeList, value); }
        }


        private void CanExecuteSaveNodeList(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private async void ExecuteSaveNodeList(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            var ret  = UnitCore.Instance.SaveInit();
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "确定";
            if (ret == false)
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "保存失败",
                UnitCore.Instance.NetEngine.Error, MessageDialogStyle.Affirmative, md);
            else
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "保存成功",
                UnitCore.Instance.NetEngine.Error, MessageDialogStyle.Affirmative, md);

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

            ACNBuilder.Pack002(NodeID.num, UnitCore.Instance.NodeInfoMap);
            var cmd = ACNProtocol.Package(false);
            var cl = new CommandLog();
            cl.DestID = NodeID.num;
            cl.SourceID = (int)ACNProtocol.SourceID;
            cl.LogTime = DateTime.Now;
            cl.CommID = 2;
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
                UnitCore.Instance.NetEngine.Error, MessageDialogStyle.Affirmative, md);
            else
            {
                var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                dialog.Title = "发送节点信息表";
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
