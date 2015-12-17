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

namespace BoonieBear.DeckUnit.Mov4500UI.Core
{
    public class Mov4500DataObserver:Observer<CustomEventArgs>
    {
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
                        buffer = e.DataBuffer.Slice(4, e.DataBufferLength - 4);
                    }
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
                        ACM4500Protocol.UwvdataPool.Add(buffer, (MovDataType) id);

                    }
                    else if (e.Mode == CallMode.USBL)
                    {
                        var pos = new Sysposition();
                        //tbd
                        datatype = "USBL";
                        ACM4500Protocol.ShipdataPool.Add(pos.Pack(), MovDataType.ALLPOST);
                        UnitCore.Instance.EventAggregator.PublishMessage(new USBLEvent(pos));

                    }
                    else if (e.Mode == CallMode.GPS)
                    {
                        //tbd
                        datatype = "GPS";
                    }
                    else if (e.Mode == CallMode.DataMode) //payload or ssb
                    {
                        switch (id)
                        {
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
                            default:
                                return;
                        }
                        if (id == (int) ModuleType.Req)
                        {
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
                        }
                    }
                    UnitCore.Instance.MovTraceService.Save(datatype, buffer); //保存上面全部数据类型
                    if (e.Mode == CallMode.DataMode)
                    {
                        if (id == (int) ModuleType.SSB)
                        {
                            UnitCore.Instance.Wave.Dispatcher.Invoke(new Action(() =>
                            {
                                UnitCore.Instance.Wave.Display(buffer);
                            }));

                            return;
                        }
                        if (id == (int) ModuleType.FH)
                        {
                            LogHelper.WriteLog("收到跳频数据:" + Encoding.Default.GetString(buffer, 0, 8));
                            if (ACM4500Protocol.ParseFH(buffer))
                            {
                                UnitCore.Instance.EventAggregator.PublishMessage(new MovDataEvent(
                                    ModuleType.FH, ACM4500Protocol.Results));
                            }
                            return;
                        }
                        UnitCore.Instance.ACMMutex.WaitOne();
                        var ret = ACM4500Protocol.DecodeACNData(buffer);
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
                        pro.Parse(buffer); //解析UDP数据
                        //tbd
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
