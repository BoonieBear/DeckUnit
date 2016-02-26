using System;
using System.Net.Sockets;
using System.Text;
using BoonieBear.DeckUnit.ACMP;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using BoonieBear.DeckUnit.Mov4500UI.Helpers;
using DevExpress.Xpf.Core;
using Microsoft.Win32;
using ImageProc;
using BoonieBear.DeckUnit.Device.USBL;
namespace BoonieBear.DeckUnit.Mov4500UI.Core
{
    public class Mov4500DataObserver:Observer<CustomEventArgs>
    {
        USBLParser usblParser = new USBLParser();
        public async void Handle(object sender, CustomEventArgs e)
        {
            string datatype = "";
            if (e.ParseOK)
            {
                IProtocol pro = null;
                try
                {
                    int id = 0;
                    byte[] buffer = null;
                    
                    if (e.Mode != CallMode.NoneMode)
                    {
                        id = BitConverter.ToUInt16(e.DataBuffer, 0);
                        if (e.Mode == CallMode.Sail)
                        {
                            buffer = new byte[e.DataBufferLength - 2];
                            Buffer.BlockCopy(e.DataBuffer, 2, buffer, 0, e.DataBufferLength - 2);
                        }
                        else
                        {
                            buffer = new byte[e.DataBufferLength - 4];
                            Buffer.BlockCopy(e.DataBuffer, 4, buffer, 0, e.DataBufferLength - 4);
                        }
                        
                    }
                    else//shell
                    {
                        string shell = e.Outstring;
                        if (shell.Contains("ver\r\n"))
                        {
                            UnitCore.Instance.Version = shell.Substring(shell.LastIndexOf("ver\r\n") + 5);
                           
                        }
                        if (shell.Contains("浮点处理器"))
                        {
                            UnitCore.Instance.Version += shell;
                            if (shell.Contains("/>"))
                                UnitCore.Instance.Version  = UnitCore.Instance.Version.Replace("/>","");
                        }
                    }
                    //类型标志
                    if (e.Mode == CallMode.Sail) //水下航控或ADCP或BP
                    {
                        switch (id)
                        {
                            case (int) ExchageType.SUBPOST:
                                pro = new Subposition();
                                datatype = "SAILTOACOUSTIC";

                                break;
                            case (int) ExchageType.BP:
                                pro = new Bpdata();
                                datatype = "BP";

                                break;
                            case (int) ExchageType.CTD:
                                pro = new Ctddata();
                                datatype = "SAILTOACOUSTIC";

                                break;
                            case (int) ExchageType.LIFESUPPLY:
                                pro = new Lifesupply();
                                datatype = "SAILTOACOUSTIC";

                                break;
                            case (int) ExchageType.ENERGY:
                                pro = new Energysys();
                                datatype = "SAILTOACOUSTIC";

                                break;
                            case (int) ExchageType.ALERT:
                                pro = new Alertdata();
                                datatype = "SAILTOACOUSTIC";

                                break;

                            case (int) ExchageType.ADCP:
                                pro = new Adcpdata();
                                datatype = "ADCP";

                                break;
                            default:
                                throw new Exception("未知的UDP数据类型");
                                break;
                        }     

                    }
                    else if (e.Mode == CallMode.USBL)
                    {
                        datatype = "USBL";
                    }
                    else if (e.Mode == CallMode.GPS)
                    {
                        datatype = "GPS";
                    }
                    else if (e.Mode == CallMode.DataMode) //payload or ssb
                    {
                        switch (id)
                        {
                            case (int) ModuleType.SSBNULL:
                            case (int) ModuleType.SSB:
                                datatype = "RECVVOICE";

                                break;
                            case (int) ModuleType.MFSK:
                                datatype = "RECVFSK";

                                break;
                            case (int) ModuleType.MPSK:
                                datatype = "RECVPSK";

                                break;
                            case (int) ModuleType.FH:
                                datatype = "FH";
                                break;
                            case (int) ModuleType.Req:
                                LogHelper.WriteLog("收到DSP请求");
                                if (UnitCore.Instance.WorkMode == MonitorMode.SUBMARINE)
                                {
                                    if (ACM4500Protocol.UwvdataPool.HasImg)
                                    {
                                        var data = ACM4500Protocol.PackData(ModuleType.MPSK);
                                        LogHelper.WriteLog("发送MPSK");
                                        UnitCore.Instance.NetCore.Send((int) ModuleType.MPSK, data);
                                        UnitCore.Instance.MovTraceService.Save("XMTPSK", data);
                                        return;
                                    }
                                }
                                var fskdata = ACM4500Protocol.PackData(ModuleType.MFSK);
                                LogHelper.WriteLog("发送MFSK");
                                UnitCore.Instance.NetCore.Send((int) ModuleType.MFSK, fskdata);
                                UnitCore.Instance.MovTraceService.Save("XMTFSK", fskdata);
                                return;
                            default:
                                return;
                        }
                    }
                    //保持数据
                    if (id == (int)ModuleType.FH)
                        UnitCore.Instance.MovTraceService.Save(datatype, Encoding.Default.GetString(buffer)); //FH
                    else
                    {
                        if (id == (int) ModuleType.SSBNULL)
                        {
                            int length = BitConverter.ToUInt16(buffer,0);
                            buffer = new byte[length*2];
                            Array.Clear(buffer,0,length*2);
                        }
                        UnitCore.Instance.MovTraceService.Save(datatype, buffer); //保存上面除FH全部数据类型
                    }
                    //开始处理
                    if (e.Mode == CallMode.DataMode)
                    {
                        if (id == (int)ModuleType.SSBNULL)
                            return;
                        if (id == (int) ModuleType.SSB)
                        {
                            if (UnitCore.Instance.Wave != null)
                            {
                                UnitCore.Instance.Wave.Dispatcher.Invoke(new Action(() =>
                                {
                                    UnitCore.Instance.Wave.Display(buffer);
                                }));
                            }
                            return;
                        }
                        if (id == (int) ModuleType.FH)
                        {
                            LogHelper.WriteLog("收到跳频数据:" + Encoding.Default.GetString(buffer));
                            if (ACM4500Protocol.ParseFH(buffer))
                            {
                                UnitCore.Instance.EventAggregator.PublishMessage(new MovDataEvent(
                                    ModuleType.FH, ACM4500Protocol.Results));
                            }
                            return;
                        }
                        UnitCore.Instance.ACMMutex.WaitOne();
                        var ret = ACM4500Protocol.DecodeACNData(buffer, (ModuleType)Enum.Parse(typeof(ModuleType), id.ToString()));
                        //var ret = buffer;
                        if (ret != null)
                        {
                        
                            switch (id)
                            {
                                case (int) ModuleType.MFSK:
                                    LogHelper.WriteLog("收到MFSK数据");
                                    UnitCore.Instance.MovTraceService.Save("FSKSRC", ret);
                                    if (ACM4500Protocol.ParseFSK(ret))
                                    {
                                        UnitCore.Instance.EventAggregator.PublishMessage(new MovDataEvent(
                                            ModuleType.MFSK, ACM4500Protocol.Results));
                                    }
                                    break;
                                case (int) ModuleType.MPSK:
                                    LogHelper.WriteLog("收到MPSK数据");
                                    UnitCore.Instance.MovTraceService.Save("PSKSRC", ret);
                                    var jpcdata = ACM4500Protocol.ParsePSK(ret);
                                    if (jpcdata != null) //全部接收
                                    {
                                        UnitCore.Instance.MovTraceService.Save("PSKJPC", jpcdata);
                                        if (Jp2KConverter.LoadJp2k(jpcdata))
                                        {
                                            var imgbuf =
                                                Jp2KConverter.SaveImg(UnitCore.Instance.MovConfigueService.MyExecPath +
                                                                      "\\" + "decode.jpg");
                                            if (imgbuf != null)
                                            {
                                                UnitCore.Instance.MovTraceService.Save("IMG", imgbuf);
                                                ACM4500Protocol.Results.Add(MovDataType.IMAGE, imgbuf);
                                            }
                                            UnitCore.Instance.EventAggregator.PublishMessage(new MovDataEvent(
                                                ModuleType.MPSK, ACM4500Protocol.Results));
                                        }
                                    }
                                    break;
                            }

                        }
                        UnitCore.Instance.ACMMutex.ReleaseMutex();
                    }
                    else if (e.Mode == CallMode.Sail)
                    {
                        ACM4500Protocol.UwvdataPool.Add(buffer, (MovDataType)id);
                        pro.Parse(buffer); //解析UDP数据
                        UnitCore.Instance.EventAggregator.PublishMessage(new SailEvent(
                                                (ExchageType)id, pro));
                    }
                    else if (e.Mode == CallMode.USBL)
                    {
                        var pos = new Sysposition();
                        if (usblParser.Parse(Encoding.ASCII.GetString(buffer)))
                        {
                            pos._ltime = usblParser.Time;
                            pos._relateX = usblParser.X;
                            pos._relateY = usblParser.Y;
                            pos._relateZ = usblParser.Z;
                            pos._shipLat = usblParser.ShipLat;
                            pos._shipLong = usblParser.ShipLng;
                            pos._shipheading = usblParser.Heading;
                            pos._shippitch = usblParser.Pitch;
                            pos._shiproll = usblParser.Roll;
                            pos._shipvel = usblParser.ShipVelocity;
                            pos._subLat = usblParser.MovLat;
                            pos._subLong = usblParser.MovLng;
                            pos._subdepth = usblParser.MovDepth;

                        }
                        ACM4500Protocol.ShipdataPool.Add(pos.Pack(), MovDataType.ALLPOST);
                        UnitCore.Instance.EventAggregator.PublishMessage(new USBLEvent(pos));

                    }
                }
                catch (Exception ex)
                {
                    UnitCore.Instance.ACMMutex.ReleaseMutex();
                    UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(ex, LogType.Both));
                }


            }
            else
            {
                if (e.Mode == CallMode.ErrMode)
                {
                    App.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(e.Ex, LogType.Both));
                    }));
                    UnitCore.Instance.NetCore.StopTCpService();
                }
            }
        }


    }
}
