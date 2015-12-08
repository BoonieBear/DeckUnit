using System;
using System.Net.Sockets;
using BoonieBear.DeckUnit.ACMP;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using DevExpress.Xpf.Core;
using Microsoft.Win32;

namespace BoonieBear.DeckUnit.Mov4500UI.Core
{
    public class Mov4500DataObserver:Observer<CustomEventArgs>
    {
        public void Handle(object sender, CustomEventArgs e)
        {
            string datatype = "";
            if (e.ParseOK)
            {
                IProtocol pro =null;
                try
                {

                    int id = BitConverter.ToInt16(e.DataBuffer, 0);
                    var buffer = e.DataBuffer.Slice(4, e.DataBufferLength - 4); //delete head
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
                            case (int) ExchageType.BSSS:
                                pro = new Bsssdata();
                                datatype = "BSSS";

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
                        }
                    }
                    UnitCore.Instance.MovTraceService.Save(datatype, buffer);//保存上面全部数据类型
                    if (e.Mode == CallMode.DataMode)
                    {
                        UnitCore.Instance.ACMMutex.WaitOne();
                        var ret = ACM4500Protocol.DecodeACNData(buffer);
                        if (ret != null)
                        {
                            switch ((int) BitConverter.ToInt16(buffer, 0))
                            {
                                case (int)ModuleType.FH:
                                    
                                    //tbd
                                    break;
                                case (int)ModuleType.SSB:

                                    //tbd
                                    break;
                                case (int)ModuleType.MFSK:
                                    UnitCore.Instance.MovTraceService.Save("FSKSRC", ret);
                                    if (ACM4500Protocol.ParseFSK(ret))
                                    {
                                        //tbd
                                    }
                                    break;
                                case (int)ModuleType.MPSK:
                                    UnitCore.Instance.MovTraceService.Save("PSKSRC", ret);
                                    var jpcdata = ACM4500Protocol.ParsePSK(ret);
                                    if (jpcdata!=null)//全部接收
                                    {
                                        UnitCore.Instance.MovTraceService.Save("PSKJPC", jpcdata);
                                        //tbd
                                    }
                                    break;
                            }

                        }
                        UnitCore.Instance.ACMMutex.ReleaseMutex();
                    }
                    else if(e.Mode==CallMode.Sail)
                    {
                        pro.Parse(buffer); //解析UDP数据
                        //tbd
                    }
                }
                catch (Exception ex)
                {
                    UnitCore.Instance.ACMMutex.ReleaseMutex();
                    UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent(ex.Message, LogType.Both));
                }
                

            }
        }


    }
}
