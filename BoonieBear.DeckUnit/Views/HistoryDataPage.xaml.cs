using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using BoonieBear.DeckUnit.ACNP;
using BoonieBear.DeckUnit.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.Events;
using BoonieBear.DeckUnit.JsonUtils;
using BoonieBear.DeckUnit.Models;
using BoonieBear.DeckUnit.ViewModels;
using MahApps.Metro.Controls;

namespace BoonieBear.DeckUnit.Views
{
    public partial class HistoryDataPage
    {
        public HistoryDataPage()
        {
            InitializeComponent();
        }

        private void FilterableListView_SelectionChanged(object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                CommandLog cl = (CommandLog)DataListView.SelectedItem;
                if (cl == null)
                    return;
                var fr = File.Open(cl.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var br = new BinaryReader(fr);
                UnitCore.Instance.AcnMutex.WaitOne();
                ACNProtocol.GetDataForParse(br.ReadBytes((int)fr.Length));
                if (ACNProtocol.Parse())
                {
                    var tree = StringListToTree.TransListToNodeWriteLineic(ACNProtocol.parselist);
                    UnitCore.Instance.AcnMutex.ReleaseMutex();
                    var datatree = new DataTreeModel(tree);
                    MainFrame mf = Application.Current.MainWindow as MainFrame;
                    if(mf==null)
                        return;
                    mf._tree.Model = datatree;
                    MainFrameViewModel.pMainFrame.DataRecvTime = cl.LogTime.ToString();
                    var flyout = mf.flyoutsControl.Items[2] as Flyout;
                    flyout.IsOpen = true;
                }
                else
                {
                    UnitCore.Instance.AcnMutex.ReleaseMutex();
                    UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(new Exception(ACNProtocol.Errormessage), LogType.Both));
                }
            }
            catch (Exception ex)
            {
                UnitCore.Instance.AcnMutex.ReleaseMutex();
                UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(ex, LogType.Both));
            }
        }

        private void DataListView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                CommandLog cl = (CommandLog)DataListView.SelectedItem;
                if (cl == null)
                    return;
                var fr = File.Open(cl.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var br = new BinaryReader(fr);
                UnitCore.Instance.AcnMutex.WaitOne();
                ACNProtocol.GetDataForParse(br.ReadBytes((int)fr.Length));
                if (ACNProtocol.Parse())
                {
                    var tree = StringListToTree.TransListToNodeWriteLineic(ACNProtocol.parselist);
                    UnitCore.Instance.AcnMutex.ReleaseMutex();
                    var datatree = new DataTreeModel(tree);
                    MainFrame mf = Application.Current.MainWindow as MainFrame;
                    if(mf==null)
                        return;
                    mf._tree.Model = datatree;
                    MainFrameViewModel.pMainFrame.DataRecvTime = cl.LogTime.ToString();
                    var flyout = mf.flyoutsControl.Items[2] as Flyout;
                    flyout.IsOpen = true;
                }
                else
                {
                    UnitCore.Instance.AcnMutex.ReleaseMutex();
                    UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(new Exception(ACNProtocol.Errormessage), LogType.Both));
                }
            }
            catch (Exception ex)
            {
                UnitCore.Instance.AcnMutex.ReleaseMutex();
                UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(ex, LogType.Both));
            }
        }

        private void CMDListView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                CommandLog cl = (CommandLog)CMDListView.SelectedItem;
                if (cl == null)
                    return;
                var fr = File.Open(cl.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var br = new BinaryReader(fr);
                UnitCore.Instance.AcnMutex.WaitOne();
                ACNProtocol.GetDataForParse(br.ReadBytes((int)fr.Length));
                if (ACNProtocol.Parse())
                {
                    var tree = StringListToTree.TransListToNodeWriteLineic(ACNProtocol.parselist);
                    UnitCore.Instance.AcnMutex.ReleaseMutex();
                    var datatree = new DataTreeModel(tree);
                    MainFrame mf = Application.Current.MainWindow as MainFrame;
                    if (mf == null)
                        return;
                    mf._tree.Model = datatree;
                    MainFrameViewModel.pMainFrame.DataRecvTime = cl.LogTime.ToString();
                    var flyout = mf.flyoutsControl.Items[2] as Flyout;
                    flyout.IsOpen = true;
                }
                else
                {
                    UnitCore.Instance.AcnMutex.ReleaseMutex();
                    UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(new Exception(ACNProtocol.Errormessage), LogType.Both));
                }
            }
            catch (Exception ex)
            {
                UnitCore.Instance.AcnMutex.ReleaseMutex();
                UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(ex, LogType.Both));
            }
        }

        private void CMDListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CommandLog cl = (CommandLog)CMDListView.SelectedItem;
                if (cl == null)
                    return;
                var fr = File.Open(cl.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var br = new BinaryReader(fr);
                UnitCore.Instance.AcnMutex.WaitOne();
                ACNProtocol.GetDataForParse(br.ReadBytes((int)fr.Length));
                if (ACNProtocol.Parse())
                {
                    var tree = StringListToTree.TransListToNodeWriteLineic(ACNProtocol.parselist);
                    UnitCore.Instance.AcnMutex.ReleaseMutex();
                    var datatree = new DataTreeModel(tree);
                    MainFrame mf = Application.Current.MainWindow as MainFrame;
                    if (mf == null)
                        return;
                    mf._tree.Model = datatree;
                    MainFrameViewModel.pMainFrame.DataRecvTime = cl.LogTime.ToString();
                    var flyout = mf.flyoutsControl.Items[2] as Flyout;
                    flyout.IsOpen = true;
                }
                else
                {
                    UnitCore.Instance.AcnMutex.ReleaseMutex();
                    UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(new Exception(ACNProtocol.Errormessage), LogType.Both));
                }
            }
            catch (Exception ex)
            {
                UnitCore.Instance.AcnMutex.ReleaseMutex();
                UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(ex, LogType.Both));
            }
        }

       
    }
}