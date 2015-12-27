using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BoonieBear.DeckUnit.ACNP;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.Events;
using BoonieBear.DeckUnit.Helps;
using BoonieBear.DeckUnit.JsonUtils;
using BoonieBear.DeckUnit.Models;
using BoonieBear.DeckUnit.UBP;
using BoonieBear.DeckUnit.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using BoonieBear.DeckUnit.BaseType;
namespace BoonieBear.DeckUnit.Core
{
    public class DeckUnitDataObserver:Observer<CustomEventArgs>
    {
        private string str;
        public async void Handle(object sender, CustomEventArgs e)
        {
            if (e.ParseOK)
            {
                if (e.Mode == CallMode.NoneMode)
                {
                    str = e.Outstring+"\n";
                    var tcpsrc = e.CallSrc as TcpClient;
                    if (tcpsrc != null) //网络shell
                    {
                        UnitCore.Instance.UnitTraceService.WriteShell(str);
                        MainFrameViewModel.pMainFrame.Shellstring+= str;
                        
                        if (MainFrameViewModel.pMainFrame.Shellstring.Length > 1024)
                        {
                            MainFrameViewModel.pMainFrame.Shellstring.Remove(0, 256);
                        }
                    }
                     var udpsrc = e.CallSrc as UdpClient;
                    if (udpsrc != null) //trace
                    {
                        UnitCore.Instance.UnitTraceService.WriteTrace(str);
                        MainFrameViewModel.pMainFrame.Dispatch(() =>MainFrameViewModel.pMainFrame.TraceCollMt.Add(str));
                        
                        if (MainFrameViewModel.pMainFrame.TraceCollMt.Count > 200)
                        {
                            //lock (MainFrameViewModel.pMainFrame.TraceCollMt.SyncRoot)
                            {
                                MainFrameViewModel.pMainFrame.Dispatch(() =>MainFrameViewModel.pMainFrame.TraceCollMt.RemoveAt(0));
                            }
                        }
                    }
                }
                else if (e.Mode == CallMode.LoaderMode||e.Mode == CallMode.AnsMode)
                {
                    var src = e.CallSrc as SerialPort;
                    if (src != null) //loader/Ans mode, 没有存储需求
                    {
                        str = e.Outstring + "\n";
                        MainFrameViewModel.pMainFrame.Serialstring += str; 
                        if (MainFrameViewModel.pMainFrame.Serialstring.Length > 4096)
                            MainFrameViewModel.pMainFrame.Serialstring.Remove(0, 1024);
                    }
                }
                else if (e.Mode == CallMode.DataMode||e.ID==170)//网络数据或串口转发
                {
                    var bytes = new byte[e.DataBufferLength-4];
                    Buffer.BlockCopy(e.DataBuffer, 4, bytes, 0, e.DataBufferLength-4);
                    switch (BitConverter.ToUInt16(e.DataBuffer, 0))
                    {
                        case 0xEE01:
                            ParseData(bytes);
                            break;
                        case 0xEDED:
                            UnitCore.Instance.UnitTraceService.CloseAD();
                            break;
                        case 0xAD01:
                        case 0xAD02:
                        case 0xAD03:
                        case 0xAD04:
                            UnitCore.Instance.UnitTraceService.SaveAD(e.DataBuffer);
                            UnitCore.Instance.EventAggregator.PublishMessage(
                                new UpdateADByteCount((int)UnitCore.Instance.UnitTraceService.GetADCount()));
                            break;
                        case (int)PackType.Ack:
                        case (int)PackType.Data:
                            string err;
                            var ret = DeckDataProtocol.ParseData(e.DataBuffer, out err);
                            if (TaskStage.Failed == ret)
                            {
                                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent(err, LogType.Both));
                            }
                            else if (ret == TaskStage.Continue)//不需要发reply包
                            {
                                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("收到数据包", LogType.OnlyLog));
                            }
                            else if (ret == TaskStage.Finish)
                            {
                                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("数据接收完毕！",LogType.Both));
                            }
                            break;
                        case (int)PackType.Ans:
                            string error;
                            switch ((TaskState)BitConverter.ToInt16(bytes,0))
                            {       
                                
                                    case TaskState.OK://查询到数据
                                        error = "";
                                        string ans = BitConverter.ToInt16(bytes, 0).ToString();
                                        string ping = BitConverter.ToInt16(bytes, 2).ToString();
                                        string length = BitConverter.ToInt32(bytes, 4).ToString();
                                        App.Current.Dispatcher.Invoke(new Action(() =>
                                        {
                                            var md = new MetroDialogSettings();
                                            md.AffirmativeButtonText = "确定";
                                            MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(
                                                MainFrameViewModel.pMainFrame, "查询数据成功",
                                                "应答ID：" + ans + "\n" + "Ping= " + ping + "\n" +
                                                "数据长度= " + length, MessageDialogStyle.Affirmative,
                                                md);
                                        }));
                                        break;
                                default:
                                    string errno="";
                                    if ((TaskState)BitConverter.ToInt16(bytes, 0)==TaskState.DATA_FAILED)
                                        errno = "检索时间段内无数据";
                                    if ((TaskState)BitConverter.ToInt16(bytes, 0)== TaskState.NO_FILE)
                                        errno = "该时段没有数据";
                                    if ((TaskState)BitConverter.ToInt16(bytes, 0)== TaskState.RECV_FAILED)
                                        errno = "接收数据出错";
                                    if ((TaskState)BitConverter.ToInt16(bytes, 0)== TaskState.WAKE_FAILED)
                                        errno = "唤醒失败";
                                    if ((TaskState)BitConverter.ToInt16(bytes, 0)==TaskState.INVALID_CMD)
                                        errno = "命令解析失败";
                                    if ((TaskState)BitConverter.ToInt16(bytes, 0)== TaskState.SD_ERR)
                                        errno = "SD卡故障";
                                    var ms = new MetroDialogSettings();
                                    ms.AffirmativeButtonText = "确定";
                                    MainFrameViewModel.pMainFrame.Dispatch(()=>MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(
                                        MainFrameViewModel.pMainFrame, "查询数据失败", errno, MessageDialogStyle.Affirmative,
                                        ms));
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    
                }
                else if (e.Mode == CallMode.CommData)
                {
                    var bytes = new byte[e.DataBufferLength];
                    Buffer.BlockCopy(e.DataBuffer, 0, bytes, 0, e.DataBufferLength);
                    if (e.ID == 12)
                        UnitCore.Instance.EventAggregator.PublishMessage(new NodeStatusInfo(e.Outstring, bytes));
                }
            }
            else
            {
                Debug.WriteLine(e.ErrorMsg);
            }
        }

        private void ParseData(byte[] bytes)
        {
            UnitCore.Instance.AcnMutex.WaitOne();
            ACNProtocol.GetDataForParse(bytes);
            try
            {
                
                if (ACNProtocol.Parse())
                {
                    var savedata = new CommandLog();
                    foreach (var parsestr in ACNProtocol.parselist)//取最后一包的ID,第一包是路径包
                    {
                        if (parsestr[1] == "ID")
                            savedata.CommID = int.Parse(parsestr[2]);
                        if (parsestr[1] == "起始源地址")
                            savedata.SourceID = int.Parse(parsestr[2]);
                        if (parsestr[1] == "目的地址")
                            savedata.DestID = int.Parse(parsestr[2]);
                        if (savedata.CommID == 101) //ping
                        {
                            UnitCore.Instance.EventAggregator.PublishMessage(new PingNotifyEvent(parsestr[3]));
                            UnitCore.Instance.EventAggregator.PublishMessage(new StatusNotify("回环测试", "收到回环测试数据，请查看测试页面", NotifyLevel.Info));
                        }
                    }
                    savedata.LogTime = DateTime.Now;
                    savedata.Type = true;
                    if (UnitCore.Instance.UnitTraceService.Save(savedata, bytes))
                    {
                        savedata.FilePath = UnitCore.Instance.UnitTraceService.FileName;
                    }
                    MainFrameViewModel.pMainFrame.Dispatch(() => MainFrameViewModel.pMainFrame.DataCollMt.Add(savedata));
                    
                    MainFrameViewModel.pMainFrame.RecvMessage ++;
                    MainFrameViewModel.pMainFrame.DataRecvTime = savedata.LogTime.ToString();
                    
                }
                UnitCore.Instance.AcnMutex.ReleaseMutex();
            }
            catch (Exception ex)
            {
                UnitCore.Instance.AcnMutex.ReleaseMutex();
                UnitCore.Instance.EventAggregator.PublishMessage(new StatusNotify("数据解析", ex.Message, NotifyLevel.Warning));
            }
 
        }
    }
}
