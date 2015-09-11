using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using BoonieBear.DeckUnit.Models;
namespace BoonieBear.DeckUnit.ViewModels.CommandViewModel
{
    public class NodeComSchemaViewModel : ViewModelBase
    {
        public override void Initialize()
        {
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            SendCMD = RegisterCommand(ExecuteSendCMD, CanExecuteSendCMD, true);
            ID = new List<FilterItem>();
            for (int i = 1; i < 64; i++)
            {
                ID.Add(new FilterItem(i.ToString())); ;
            }
            NodeID = ID[0];
            List1 = new ObservableCollection<Item>()
                        {
                            new Item(1, "01#制式"),
                            new Item(2, "02#制式"),
                            new Item(3, "03#制式"),
                            new Item(4, "04#制式"),
                            new Item(5, "05#制式"),
                            new Item(6, "06#制式"),
                            new Item(7, "07#制式"),
                            new Item(8, "08#制式"),
                            new Item(9, "09#制式"),
                            new Item(10, "10#制式"),
                            new Item(11, "11#制式"),
                            new Item(12, "12#制式"),
                            new Item(13, "13#制式"),
                            new Item(14, "14#制式"),
                            new Item(15, "15#制式"),
                            new Item(16, "16#制式"),
                        };
            List2 = new ObservableCollection<Item>();

            CopyAllItemsCommand = RegisterCommand(ExecuteCopyAllItemsCommand, CanExecuteCopyAllItemsCommand, true);
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
        #region Property CurrentItem1

        /// <summary>
        /// Gets or sets CurrentItem1
        /// </summary>
        public Item CurrentItem1
        {
            get { return GetPropertyValue(() => CurrentItem1); }
            set
            {
                SetPropertyValue(() => CurrentItem1, value);
                if (value != null)
                {
                    CurrentItem1 = null;
                    List2.Add(value);
                    List1.Remove(value);

                    OnPropertyChanged(() => List2);
                    OnPropertyChanged(() => List1);
                }
            }
        }

        #endregion

        #region Property CurrentItem2

        /// <summary>
        /// Gets or sets CurrentItem2
        /// </summary>
        public Item CurrentItem2
        {
            get { return GetPropertyValue(() => CurrentItem2); }
            set
            {
                SetPropertyValue(() => CurrentItem2, value);
                if (value != null)
                {
                    CurrentItem2 = null;
                    List1.Add(value);
                    List2.Remove(value);

                    OnPropertyChanged(() => List2);
                    OnPropertyChanged(() => List1);
                }
            }
        }

        #endregion

        #region Property List1

        /// <summary>
        /// Gets or sets List1
        /// </summary>
        public ObservableCollection<Item> List1
        {
            get { return GetPropertyValue(() => List1); }
            set { SetPropertyValue(() => List1, value); }
        }

        #endregion

        #region Property List2

        /// <summary>
        /// Gets or sets List2
        /// </summary>
        public ObservableCollection<Item> List2
        {
            get { return GetPropertyValue(() => List2); }
            set { SetPropertyValue(() => List2, value); }
        }

        #endregion

        #region CopyAllItems Command

        
        public ICommand CopyAllItemsCommand
        {
            get { return GetPropertyValue(() => CopyAllItemsCommand); }
            set { SetPropertyValue(() => CopyAllItemsCommand, value); }
        }

        
        public void CanExecuteCopyAllItemsCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        
        public void ExecuteCopyAllItemsCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            foreach (var item in List1)
                List2.Add(item);

            List1.Clear();
            OnPropertyChanged(() => List1);
            OnPropertyChanged(() => List2);
        }

        #endregion
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
            bool[] bit = new bool[16];
            var er = List2.GetEnumerator();
            while (er.MoveNext())
            {
                bit[er.Current.SortOrder - 1] = true;
            }
            ACNBuilder.Pack142(NodeID.num, new BitArray(bit));
            var cmd = ACNProtocol.Package(false);
            var cl = new CommandLog();
            cl.DestID = NodeID.num;
            cl.SourceID = (int)ACNProtocol.SourceID;
            cl.LogTime = DateTime.Now;
            cl.CommID = 142;
            cl.Type = false;
            UnitCore.Instance.UnitTraceService.Save(cl, cmd);
            var result = UnitCore.Instance.NetEngine.SendCMD(cmd);
            await result;
            var ret = result.Result;
            IsProcessing = false;
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "好的";
            if (ret == false)
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "发送失败",
                UnitCore.Instance.NetEngine.Error,MessageDialogStyle.Affirmative,md);
            else
            {
                var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                dialog.Title = "设备参数命令";
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
