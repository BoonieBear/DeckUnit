using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Net.Sockets;
using BoonieBear.DeckUnit.ACNP;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.Events;
using BoonieBear.DeckUnit.JsonUtils;
using BoonieBear.DeckUnit.Models;
using BoonieBear.DeckUnit.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BoonieBear.DeckUnit.Core
{
    public class DeckUnitDataObserver:Observer<CustomEventArgs>
    {
        private string str;
        public void Handle(object sender, CustomEventArgs e)
        {
            if (e.ParseOK)
            {
                if (e.Mode == CallMode.NoneMode)
                {
                    str = e.Outstring;
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
                        MainFrameViewModel.pMainFrame.TraceCollMt.Add(str);
                        if (MainFrameViewModel.pMainFrame.TraceCollMt.Count > 200)
                        {
                            //lock (MainFrameViewModel.pMainFrame.TraceCollMt.SyncRoot)
                            {
                                MainFrameViewModel.pMainFrame.TraceCollMt.RemoveAt(0);
                            }
                        }
                    }
                }
                else if (e.Mode == CallMode.LoaderMode)
                {
                    var src = e.CallSrc as SerialPort;
                    if (src != null) //loader mode, 没有存储需求
                    {
                        MainFrameViewModel.pMainFrame.Serialstring += str;
                        if (MainFrameViewModel.pMainFrame.Serialstring.Length > 4096)
                            MainFrameViewModel.pMainFrame.Serialstring.Remove(0, 1024);
                    }
                }
                else if (e.Mode == CallMode.DataMode||e.ID==170)//网络数据或串口转发
                {
                    var bytes = new byte[e.DataBufferLength];
                    Buffer.BlockCopy(e.DataBuffer, 0, bytes, 0, e.DataBufferLength);
                    //UnitCore.Instance.UnitTraceService.Save(bytes);//TBD
                    ACNProtocol.GetDataForParse(bytes);
                    try
                    {
                        if (ACNProtocol.Parse())
                        {
                            var nodetree = StringListToTree.TransListToNodeWriteLineic(ACNProtocol.parselist);
                            var newtree = StringListToTree.RemoveFatherPointer(nodetree);
                            var jsonstr = JsonConvert.SerializeObject(newtree);
                            str = jsonstr;
                            var savedata = new CommandLog();
                            var javascript = (JObject)JsonConvert.DeserializeObject(str);
                            var json = (JObject)javascript["数据区"];
                            savedata.LogTime = DateTime.Now;
                            savedata.CommID = Int16.Parse(json["ID"].ToString());
                            savedata.SourceID = Int16.Parse(((JObject)javascript["块"])["起始源地址"].ToString());
                            string nodeid = savedata.SourceID.ToString("D2");
                            savedata.DestID = Int16.Parse(((JObject)javascript["块"])["目的地址"].ToString());
                            if (UnitCore.Instance.UnitTraceService.Save(savedata, bytes))
                                savedata.FilePath = UnitCore.Instance.UnitTraceService.FileName;
                            MainFrameViewModel.pMainFrame.DataCollMt.Add(savedata);
                        }
                    }
                    catch (Exception ex)
                    {
                        UnitCore.Instance.EventAggregator.PublishMessage(new StatusNotify("数据解析",ex.Message,NotifyLevel.Warning));
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
    }
}
