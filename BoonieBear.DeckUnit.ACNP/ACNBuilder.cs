using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using TinyMetroWpfLibrary.Utility;
using BoonieBear.DeckUnit.ACNP;
using BoonieBear.DeckUnit.BaseType;
namespace BoonieBear.DeckUnit.ACNP
{
    /// <summary>
    /// 构建ACN命令类
    /// </summary>
    public class ACNBuilder
    {
        public static void Pack002(int ID, Hashtable NodeInfo)
        {
            int nodenum = NodeInfo.Keys.Count;
            int[] dat = new int[1];
            ACNProtocol.InitForPack(nodenum * 115 + 6 + 20);
            dat[0] = 2;
            ACNProtocol.OutPutIntBit(dat, 8);
            dat[0] = nodenum * 115 + 6 + 20;
            ACNProtocol.OutPutIntBit(dat, 12);
            dat[0] = nodenum;
            ACNProtocol.OutPutIntBit(dat, 6);//节点数
            foreach (string nodename in NodeInfo.Keys)
            {
                ACNProtocol.OutPutArrayBit((BitArray)NodeInfo[nodename]);
            }
            ACNProtocol.AddPool(ID);
        }

        public static void Pack003(int ID, List<NeiborNodeinfo> NeiborNodeLst)
        {
            int[] dat = new int[1];
            int nodenum = NeiborNodeLst.Count;
            ACNProtocol.InitForPack(nodenum * 24 + 4 + 20);
            dat[0] = 3;
            ACNProtocol.OutPutIntBit(dat, 8);
            dat[0] = nodenum * 24 + 4 + 20;
            ACNProtocol.OutPutIntBit(dat, 12);
            dat[0] = nodenum;
            ACNProtocol.OutPutIntBit(dat, 4);//邻节点数
            for (int i = 0; i < nodenum; i++)
            {
                dat[0] = NeiborNodeLst[i].NodeID;
                ACNProtocol.OutPutIntBit(dat, 6);//邻节点
                dat[0] = (int)NeiborNodeLst[i].Distance*5;
                ACNProtocol.OutPutIntBit(dat, 16);//距离
                dat[0] = NeiborNodeLst[i].ChannelEstimate;
                ACNProtocol.OutPutIntBit(dat, 2);//评价
            }
            ACNProtocol.AddPool(ID);
        }

        public static void Pack004(int ID, List<NetworkList> NetLst)//ID=0 广播
        {
            int[] dat = new int[1];
            int nodenum = NetLst.Count;
            ACNProtocol.InitForPack(nodenum * 24 + 4 + 20);
            dat[0] = 3;
            ACNProtocol.OutPutIntBit(dat, 8);
            dat[0] = nodenum * 30 + 4 + 20;
            ACNProtocol.OutPutIntBit(dat, 12);
            dat[0] = nodenum;
            ACNProtocol.OutPutIntBit(dat, 4);
            for (int i = 0; i < nodenum; i++)
            {
                dat[0] = NetLst[i].SourceNodeID;
                ACNProtocol.OutPutIntBit(dat, 6);
                dat[0] = NetLst[i].DestinationNodeID;
                ACNProtocol.OutPutIntBit(dat, 6);
                dat[0] = (int)NetLst[i].Distance*5;
                ACNProtocol.OutPutIntBit(dat, 16);//距离
                dat[0] = NetLst[i].ChannelEstimate;
                ACNProtocol.OutPutIntBit(dat, 2);//评价
            }
            ACNProtocol.AddPool(ID);
        }

        public static void Pack006(int ID,List<RourteList> rourte)
        {
            int nodenum = rourte.Count;
            int[] dat = new int[1];
            if (rourte.Count > 0)
            {
                ACNProtocol.InitForPack(nodenum*33 + 6 + 20);
                dat[0] = 6;
                ACNProtocol.OutPutIntBit(dat, 8);
                dat[0] = nodenum*33 + 6 + 20;
                ACNProtocol.OutPutIntBit(dat, 12);
                dat[0] = nodenum;
                ACNProtocol.OutPutIntBit(dat, 6); //路由条数
                for (int i = 0; i < nodenum; i++)
                {
                    dat[0] = rourte[i].DestinationNodeID;
                    ACNProtocol.OutPutIntBit(dat, 6); //目标节点
                    dat[0] = rourte[i].NextNodeID;
                    ACNProtocol.OutPutIntBit(dat, 6); //下一跳地址
                    dat[0] = rourte[i].Hop; //跳数
                    ACNProtocol.OutPutIntBit(dat, 4); //跳数
                    dat[0] = rourte[i].DestSerial;
                    ACNProtocol.OutPutIntBit(dat, 15);
                    dat[0] = rourte[i].RouteStatus;
                    ACNProtocol.OutPutIntBit(dat, 2);
                }
                ACNProtocol.AddPool(ID);
            }
        }

        //路径安排
        public static void Pack008(int ID,string[] nodename)
        {
            int[] dat = new int[1];
            int nodenum = nodename.Length;
            ACNProtocol.InitForPack(nodenum * 6 + 20);
            dat[0] = 8;
            ACNProtocol.OutPutIntBit(dat, 8);
            dat[0] = nodenum * 6 + 20;
            ACNProtocol.OutPutIntBit(dat, 12);
            for (int i = 0; i < nodenum; i++)
            {
                dat[0] = int.Parse(nodename[i]);
                ACNProtocol.OutPutIntBit(dat, 6);//节点
            }
            ACNProtocol.AddPool(ID);
        }

        public static void Ping(int ID,string txt)
        {
            int[] dat = new int[1];
            byte[] bstr = System.Text.Encoding.Default.GetBytes(txt);
            BitArray bta = new BitArray(bstr);
            ACNProtocol.InitForPack(bta.Length + 20);
            int[] b = new int[1];
            b[0] = 101;
            ACNProtocol.OutPutIntBit(b, 8);
            b[0] = bta.Length + 20;
            ACNProtocol.OutPutIntBit(b, 12);
            ACNProtocol.OutPutArrayBit(bta);
            ACNProtocol.AddPool(ID);
        }
        public static void Pack103(int ID)
        {
            int[] dat = new int[1];
            ACNProtocol.InitForPack(20);
            dat[0] = 103;
            ACNProtocol.OutPutIntBit(dat, 8);
            dat[0] = 20;
            ACNProtocol.OutPutIntBit(dat, 12);
            ACNProtocol.AddPool(ID);
        }
        public static void Pack105(int ID)
        {
            int[] dat = new int[1];
            ACNProtocol.InitForPack(20);
            dat[0] = 105;
            ACNProtocol.OutPutIntBit(dat, 8);
            dat[0] = 20;
            ACNProtocol.OutPutIntBit(dat, 12);
            ACNProtocol.AddPool(ID);
        }
        public static void Pack107(int ID, bool Rebuild)
        {
            int[] dat = new int[1];
            ACNProtocol.InitForPack(21);
            dat[0] = 107;
            ACNProtocol.OutPutIntBit(dat, 8);
            dat[0] = 21;
            ACNProtocol.OutPutIntBit(dat, 12);
            dat[0] = 0;//默认值
            if (Rebuild)
                dat[0] = 1;
            ACNProtocol.OutPutIntBit(dat, 1);
            ACNProtocol.AddPool(ID);
        }
        public static void Pack109(int ID)
        {
            int[] dat = new int[1];
            ACNProtocol.InitForPack(20);
            dat[0] = 109;
            ACNProtocol.OutPutIntBit(dat, 8);
            dat[0] = 20;
            ACNProtocol.OutPutIntBit(dat, 12);
            ACNProtocol.AddPool(ID);
        }
        public static void Pack111(int ID, bool Rebuild)
        {
            int[] dat = new int[1];
            ACNProtocol.InitForPack(21);
            dat[0] = 111;
            ACNProtocol.OutPutIntBit(dat, 8);
            dat[0] = 21;
            ACNProtocol.OutPutIntBit(dat, 12);
            dat[0] = 0;//默认值
            if (Rebuild)
                dat[0] = 1;
            ACNProtocol.OutPutIntBit(dat, 1);
            ACNProtocol.AddPool(ID);
        }
        public static void Pack113(int ID)
        {
            int[] dat = new int[1];
            ACNProtocol.InitForPack(20);
            dat[0] = 113;
            ACNProtocol.OutPutIntBit(dat, 8);
            dat[0] = 20;
            ACNProtocol.OutPutIntBit(dat, 12);
            ACNProtocol.AddPool(ID);
        }
        public static void Pack115(int ID,int CommIndex)
        {
            int[] dat = new int[1];
            ACNProtocol.InitForPack(28);
            dat[0] = 115;
            ACNProtocol.OutPutIntBit(dat, 8);
            dat[0] = 28;
            ACNProtocol.OutPutIntBit(dat, 12);
            dat[0] = 2;//默认值
            if (CommIndex != -1)
                dat[0] = CommIndex;
            ACNProtocol.OutPutIntBit(dat, 8);
            ACNProtocol.AddPool(ID);
        }
        public static void Pack117(int ID, int CommIndex)
        {
            int[] dat = new int[1];
            ACNProtocol.InitForPack(28);
            dat[0] = 117;
            ACNProtocol.OutPutIntBit(dat, 8);
            dat[0] = 28;
            ACNProtocol.OutPutIntBit(dat, 12);
            dat[0] = 2;//默认值
            if (CommIndex != -1)
                dat[0] = CommIndex;
            ACNProtocol.OutPutIntBit(dat, 8);
            ACNProtocol.AddPool(ID);
        }

        public static void Pack119(int ID,int CommIndex, bool bHex, string str)
        {
            int[] dat = new int[1];
            if (bHex)
            {
                byte[] end = StringHexConverter.ConvertHexToChar(str);
                int len = 20 + 8 + end.Length * 8;
                ACNProtocol.InitForPack(len);
                dat[0] = 119;
                ACNProtocol.OutPutIntBit(dat, 8);
                dat[0] = len;
                ACNProtocol.OutPutIntBit(dat, 12);
                dat[0] = 2;//默认值
                if (CommIndex == 0)
                    dat[0] = 2;
                if (CommIndex == 1)
                    dat[0] = 3;
                ACNProtocol.OutPutIntBit(dat, 8);
                for (int i = 0; i < end.Length; i++)
                {
                    dat[0] = end[i];
                    ACNProtocol.OutPutIntBit(dat, 8);
                }   
            }
            else
            {
                int arraylen = str.Length;//int[] 长度
                int len = 20 + 8 + arraylen * 8;
                ACNProtocol.InitForPack(len);
                dat[0] = 119;
                ACNProtocol.OutPutIntBit(dat, 8);
                dat[0] = len;
                ACNProtocol.OutPutIntBit(dat, 12);
                dat[0] = 2;//默认值
                if (CommIndex == 0)
                    dat[0] = 2;
                if (CommIndex == 1)
                    dat[0] = 3;
                ACNProtocol.OutPutIntBit(dat, 8);
                byte[] para = Encoding.Default.GetBytes(str);
                for (int i = 0; i < arraylen; i++)
                {
                    dat[0] = para[i];
                    ACNProtocol.OutPutIntBit(dat, 8);
                }
            }
            ACNProtocol.AddPool(ID);
        }
        public static void Pack121(int ID)
        {
            int[] dat = new int[1];
            ACNProtocol.InitForPack(20);
            dat[0] = 121;
            ACNProtocol.OutPutIntBit(dat, 8);
            dat[0] = 20;
            ACNProtocol.OutPutIntBit(dat, 12);
            ACNProtocol.AddPool(ID);
        }
        //通信制式开关
        public static void Pack142(int ID, BitArray baCommType)
        {
            int[] dat = new int[1];
            ACNProtocol.Clear();
            ACNProtocol.InitForPack(20 + 16);
            dat[0] = 142;
            ACNProtocol.OutPutIntBit(dat, 8);
            dat[0] = 36;
            ACNProtocol.OutPutIntBit(dat, 12);
            BitArray a = new BitArray(16);
            ACNProtocol.OutPutArrayBit(baCommType);
            ACNProtocol.AddPool(ID);
        }

        //设备数据定时回传开关
        public static void Pack140(int ID, int iTimePeriod)
        {
            int[] dat = new int[1];
            ACNProtocol.InitForPack(20 + 32);
            dat[0] = 140;
            ACNProtocol.OutPutIntBit(dat, 8);

            dat[0] = 52;
            ACNProtocol.OutPutIntBit(dat, 12);

            dat[0] = iTimePeriod;
            ACNProtocol.OutPutIntBit(dat, 32);
            ACNProtocol.AddPool(ID);
        }
        //收发自动调节开关
        public static void Pack141(int ID, int EmitAmp, int ReceGain)
        {
            int[] dat = new int[1];
            ACNProtocol.InitForPack(20 + 16);

            dat[0] = 141;
            ACNProtocol.OutPutIntBit(dat, 8);
            dat[0] = 36;
            ACNProtocol.OutPutIntBit(dat, 12);
            if (EmitAmp!=0)
            {
                dat[0] = 0;
                ACNProtocol.OutPutIntBit(dat, 1);
                dat[0] = EmitAmp;
                ACNProtocol.OutPutIntBit(dat, 7);
            }
            else
            {
                dat[0] = 1;
                ACNProtocol.OutPutIntBit(dat, 1);
                dat[0] = 0;
                ACNProtocol.OutPutIntBit(dat, 7);
            }
            if (ReceGain!=0)
            {
                dat[0] = 0;
                ACNProtocol.OutPutIntBit(dat, 1);
                dat[0] = ReceGain;
                ACNProtocol.OutPutIntBit(dat, 7);
            }
            else
            {
                dat[0] = 1;
                ACNProtocol.OutPutIntBit(dat, 1);
                dat[0] = 0;
                ACNProtocol.OutPutIntBit(dat, 7);
            }
            //加入列表
            ACNProtocol.AddPool(ID);
        }

        public static void Pack200()
        {
            int[] dat = new int[1];
            ACNProtocol.InitForPack(20);
            dat[0] = 200;
            ACNProtocol.OutPutIntBit(dat, 8);
            dat[0] = 20;
            ACNProtocol.OutPutIntBit(dat, 12);
            ACNProtocol.Clear();//delete all cmd in cmd list
            ACNProtocol.AddPool(0);
        }
        /// <summary>
        /// 任务包打包
        /// </summary>
        /// <param name="task">任务包类型115</param>

        public static void PackTask(BDTask task,bool bNew, int lastpkgid)
        {
            if (bNew)
            {
                int[] dat = new int[4];
                int length = 20 + 8 + 8 + 64;//不包括参数
                if (task.CommID == 2 || task.CommID == 5)
                    length += 8*8;
                if (task.CommID == 3)
                    length += 6*8;
                ACNProtocol.InitForPack(length);
                dat[0] = 115;
                ACNProtocol.OutPutIntBit(dat, 8);
                dat[0] = length;
                ACNProtocol.OutPutIntBit(dat, 12);
                dat[0] = task.DestPort;
                ACNProtocol.OutPutIntBit(dat, 8);
                dat[0] = task.CommID;
                ACNProtocol.OutPutIntBit(dat, 8);
                var idstring = task.TaskID.ToString();
                string year = idstring.Substring(0, 4);
                dat[0] = Int16.Parse(year);
                ACNProtocol.OutPutIntBit(dat, 16);
                string month = idstring.Substring(4, 2);
                dat[0] = Int16.Parse(month);
                ACNProtocol.OutPutIntBit(dat, 8);
                string day = idstring.Substring(6, 2);
                dat[0] = Int16.Parse(day);
                ACNProtocol.OutPutIntBit(dat, 8);
                string hour = idstring.Substring(8, 2);
                dat[0] = Int16.Parse(hour);
                ACNProtocol.OutPutIntBit(dat, 8);
                string minute = idstring.Substring(10, 2);
                dat[0] = Int16.Parse(minute);
                ACNProtocol.OutPutIntBit(dat, 8);
                string second = idstring.Substring(12, 2);
                dat[0] = Int16.Parse(second);
                ACNProtocol.OutPutIntBit(dat, 8);
                string dest = idstring.Substring(14, 2);
                dat[0] = Int16.Parse(dest);
                ACNProtocol.OutPutIntBit(dat, 8);
                
                if (task.CommID == 2)
                {
                    var buf = new int[4];
                    Buffer.BlockCopy(task.ParaBytes,0,buf,0,8);
                    ACNProtocol.OutPutIntBit(buf, 64);
                    ACNProtocol.AddPool(task.DestID);
                }

                if (task.CommID == 3)
                {
                    var buf = new int[3];
                    Buffer.BlockCopy(task.ParaBytes, 0, buf, 0, 6);
                    ACNProtocol.OutPutIntBit(buf, 48);
                    ACNProtocol.AddPool(task.DestID);
                }
                if (task.CommID == 5)
                {
                    var buf = new int[4];
                    Buffer.BlockCopy(task.ParaBytes, 0, buf, 0, 8);
                    ACNProtocol.OutPutIntBit(buf, 64);
                    ACNProtocol.AddPool(task.DestID);
                }
                
            }
            else//继续
            {
                int[] dat = new int[4];
                int length = 20 + 64;//不包括参数
                if (task.ErrIdxStr != null)
                {
                    string[] split = task.ErrIdxStr.Split(';');
                    length += split.Count()*8;
                }

                ACNProtocol.InitForPack(length);
                dat[0] = 115;
                ACNProtocol.OutPutIntBit(dat, 8);
                dat[0] = length;
                ACNProtocol.OutPutIntBit(dat, 12);
                dat[0] = task.DestPort;
                ACNProtocol.OutPutIntBit(dat, 8);
                dat[0] = task.CommID;
                ACNProtocol.OutPutIntBit(dat, 8);
                var idstring = task.TaskID.ToString();
                string year = idstring.Substring(0, 4);
                dat[0] = Int16.Parse(year);
                ACNProtocol.OutPutIntBit(dat, 16);
                string month = idstring.Substring(4, 2);
                dat[0] = Int16.Parse(month);
                ACNProtocol.OutPutIntBit(dat, 8);
                string day = idstring.Substring(6, 2);
                dat[0] = Int16.Parse(day);
                ACNProtocol.OutPutIntBit(dat, 8);
                string hour = idstring.Substring(8, 2);
                dat[0] = Int16.Parse(hour);
                ACNProtocol.OutPutIntBit(dat, 8);
                string minute = idstring.Substring(10, 2);
                dat[0] = Int16.Parse(minute);
                ACNProtocol.OutPutIntBit(dat, 8);
                string second = idstring.Substring(12, 2);
                dat[0] = Int16.Parse(second);
                ACNProtocol.OutPutIntBit(dat, 8);
                string dest = idstring.Substring(14, 2);
                dat[0] = Int16.Parse(dest);
                ACNProtocol.OutPutIntBit(dat, 8);
                if (lastpkgid == -1) //尚未接收到数据，不需要添加重传包
                {
                    ACNProtocol.AddPool(task.DestID);
                }
                else
                {
                    if (task.ErrIdxStr != null)
                    {
                        string[] split = task.ErrIdxStr.Split(';');
                        foreach (var strid in split)
                        {
                            dat[0] = int.Parse(strid);
                            if(dat[0]!=0)
                                ACNProtocol.OutPutIntBit(dat, 16);    
                        }
                        
                    }
                    ACNProtocol.AddPool(task.DestID);
                }
            }
            
        }
        
    }
}
