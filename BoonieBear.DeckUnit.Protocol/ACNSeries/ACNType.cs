using System;
using System.Collections;
using System.Collections.Generic;

namespace BoonieBear.DeckUnit.Protocol.ACNSeries
{
        #region 枚举成员
        public enum DeviceAddr
        {
            无设备 = 0,
            声学所浮标 = 1,
            SBE39_TD = 16,
            SBE37_CTD = 17,
            ZJU_AUV_CTD = 18,
            IOA_ADCP = 19,
            ZJU_AUV = 101,

        }
        public enum NodeType
        {
            声学所节点 = 0,
            _715通信机 = 4,
            哈工程通信机 = 8,

        }
        public enum EmitType
        {
            PWM发射机 = 0,
            线性发射机 = 1,
        }
        public enum RouteStatus
        {
            路由有效 = 0,
            路由无效 = 1,
            路由正在修复 = 2,
            非法状态标志 = 3,

        }
        public enum ChannlValue
        {
            良好 = 0,
            较好 = 1,
            较差 = 2,
            恶劣 = 3,

        }
        #endregion

        #region 协议成员类
        //{"结束标识","分段标识","邻节点表","网络表","网络简表","路由表","路径记录","路径安排",
        //                          "路径中断","转发失败"};
        //节点信息类
        //节点ID、节点能量、节点支持的通信制式、经纬度和深度
        class Nodeinfo
        {
            public int NodeId;
            public int NodePower;
            public int CommType;
            public int Lang;
            public int Lat;
            public int depth;
            public int NodeType;
            public int RecvNum;
            public int Set1type;
            public int Set2type;
            public Nodeinfo(int id, int nodetype, int Num, int set1, int set2, int power, int type, int lang, int lat, int deep)
            {
                NodeId = id;
                NodeType = nodetype;
                RecvNum = Num;
                Set1type = set1;
                Set2type = set2;
                NodePower = power;
                CommType = type;
                Lang = lang;
                Lat = lat;
                depth = deep;
            }
            public string NodeID
            {
                get { return NodeId.ToString(); }

            }
            public string MoveType
            {
                get { return (NodeType == 1) ? "移动节点" : "静止节点"; }
            }
            public string Receiver
            {
                get { return RecvNum.ToString(); }
            }

            public string Set1Type
            {
                get
                {
                    return Enum.GetName(typeof(DeviceAddr), Set1type);
                }

            }
            public string Set2Type
            {
                get
                {
                    return Enum.GetName(typeof(DeviceAddr), Set2type);
                }
            }
            public string NodePW
            {
                get
                {
                    switch (NodePower)
                    {
                        case 0:
                            return @"<5%";
                        case 1:
                            return @"5%~20%";
                        case 2:
                            return @"20%~35%";
                        case 3:
                            return @"35%~50%";
                        case 4:
                            return @"50%~65%";
                        case 5:
                            return @"65%~80%";
                        case 6:
                            return @"80%~95%";
                        case 7:
                            return @">95%";
                    }
                    return @"NULL";

                }

            }
            public string Type
            {
                get
                {
                    string comstr = "";
                    byte[] b = BitConverter.GetBytes((short)CommType);
                    BitArray ba = new BitArray(b);
                    for (int i = 0; i < ba.Count; i++)
                    {
                        comstr += ba[i] ? "1" : "0";
                    }
                    return comstr.PadRight(16, '0');
                }

            }
            public string Langtude
            {

                get
                {
                    string str;
                    if (Lang >> 27 == 1)//西经
                    {
                        Lang &= 0x7ffffff;
                        double d = (double)Lang / 10000 / 60;
                        str = d.ToString() + "°W";
                    }
                    else//东经
                    {
                        Lang &= 0x7ffffff;
                        double d = (double)Lang / 10000 / 60;
                        str = d.ToString() + "°E";
                    }


                    return str;
                }

            }
            public string Latitude
            {
                get
                {
                    string str;
                    if (Lat >> 27 == 1)//南纬
                    {
                        Lat &= 0x7ffffff;
                        double d = (double)Lat / 10000 / 60;
                        str = d.ToString() + "°S";
                    }
                    else//北纬
                    {
                        Lat &= 0x7ffffff;
                        double d = (double)Lat / 10000 / 60;
                        str = d.ToString() + "°N";
                    }


                    return str;
                }

            }
            public string Depth
            {
                get
                {
                    return (depth * 0.5).ToString();
                }

            }


        }

        //邻节点信息类
        //邻节点ID、节点距离、信道评价
        class NeiborNodeinfo
        {
            public int NodeId;
            public int Nodedist;
            public int ChannelEstimate;

            public NeiborNodeinfo(int id, int distance, int rate)
            {
                NodeId = id;
                Nodedist = distance;
                ChannelEstimate = rate;

            }
            public int NodeID
            {
                set { NodeId = value; }
                get { return NodeId; }

            }
            public double Distance
            {
                get
                {

                    return (double)Nodedist * 0.2;

                }

            }
            public string ChanEsti
            {
                get
                {
                    switch (ChannelEstimate)
                    {
                        case 0:
                            return @"信道条件良好，可支持高速通信";
                        case 1:
                            return @"信道条件较好，可支持中速通信";
                        case 2:
                            return @"信道条件较差，需降低通信速率";
                        case 3:
                            return @"信道条件恶劣，需采用高可靠性的通信";
                        default:
                            return @"错误的状态";
                    }
                }

            }


        }

        //网络表
        //节点的连接关系
        class NetworkList
        {
            public int SourceNodeId;
            public int DestinNodeId;
            public int Nodedist;
            public int ChannelEstimate;
            public NetworkList(int sid, int did, int distance, int rate)
            {
                SourceNodeId = sid;
                DestinNodeId = did;
                Nodedist = distance;
                ChannelEstimate = rate;

            }
            public int SourceNodeID
            {
                set { SourceNodeId = value; }
                get { return SourceNodeId; }

            }
            public int DestinationNodeID
            {
                set { DestinNodeId = value; }
                get { return DestinNodeId; }

            }
            public double Distance
            {
                get
                {

                    return (double)Nodedist * 0.2;

                }

            }
            public string ChanEsti
            {
                get
                {
                    switch (ChannelEstimate)
                    {
                        case 0:
                            return @"信道条件良好，可支持高速通信";
                        case 1:
                            return @"信道条件较好，可支持中速通信";
                        case 2:
                            return @"信道条件较差，需降低通信速率";
                        case 3:
                            return @"信道条件恶劣，需采用高可靠性的通信";
                        default:
                            return @"错误的状态";
                    }
                }

            }
        }

        //路由表
        //源节点到各目标节点的路由，包括目标节点、下一跳地址和跳数
        class RourteList
        {
            public int DNodeId;
            public int NextNodeId;
            public int Hops;
            public int DestSerial;
            public int RouteStatus;
            public RourteList(int did, int nid, int hop, int Serial, int Status)
            {
                DNodeId = did;
                NextNodeId = nid;
                Hops = hop;
                DestSerial = Serial;
                RouteStatus = Status;
            }
            public int DestinationNodeID
            {
                get { return DNodeId; }
            }
            public int NextNodeID
            {
                get { return NextNodeId; }
            }
            public int Hop
            {
                get { return Hops; }
            }
            public string Status
            {
                get
                {
                    switch (RouteStatus)
                    {
                        case 0:
                            return @"路由有效";
                        case 1:
                            return @"路由无效";
                        case 2:
                            return @"路由正在修复";
                        case 3:
                            return @"非法状态标志";
                        default:
                            return @"错误的状态";
                    }


                }
            }
            public int Serial
            {
                get { return DestSerial; }
            }

        }

        //路径记录
        //记录了所在数据块已经过的节点有哪些
        class RouterWriteLine
        {
            int StartId;
            List<int> IdWriteLine;
            public RouterWriteLine(int Startid, List<int> id)
            {
                StartId = Startid;
                IdWriteLine = id;
            }
            public int BeginID
            { get { return StartId; } }
            public List<int> ViaID
            { get { return IdWriteLine; } }
        }

        //路径安排
        //“路径安排”是“起始节点”对数据块按照什么样的路径进行传输的要求
        class RouterAssign
        {
            int StartId;
            List<int> IdWriteLine;
            int EndId;
            public RouterAssign(int Startid, List<int> id, int Endid)
            {
                StartId = Startid;
                IdWriteLine = id;
                EndId = Endid;
            }
            public int BeginID
            { get { return StartId; } }
            public List<int> ViaID
            { get { return IdWriteLine; } }
            public int EndID
            { get { return EndId; } }
        }

        //路径中断
        //“路径中断”是在路径中断后，判定路径中断的节点向网关发送的错误报告
        class RouterBroken
        {
            int StartId;
            //List<int> IdWriteLine;
            int EndId;
            public RouterBroken(int Startid, List<int> id, int Endid)
            {
                StartId = Startid;
                //IdWriteLine = id;
                EndId = Endid;
            }
            public int BeginID
            { get { return StartId; } }
            //public List<int> ViaID
            //{ get { return IdWriteLine; } }
            public int EndID
            { get { return EndId; } }
        }

        //转发失败
        //“转发失败”是节点在尝试各可选路径后均无法完成转发任务后，向中继链路的上一个节点的发送的错误报告
        class TransError
        {

            List<TransErrorBlock> ErrorBlock;

            public TransError(List<TransErrorBlock> Eb)
            {
                ErrorBlock = Eb;
            }
            public List<TransErrorBlock> ErrorReport
            { get { return ErrorBlock; } }

        }

        //转发失败数据体由成对的块标识和起始源地址组成
        class TransErrorBlock
        {
            int ErrNum;
            int StartId;
            public TransErrorBlock(int num, int sid)
            {
                ErrNum = num;
                StartId = sid;
            }
            public int ERRNO
            { get { return ErrNum; } }
            public int BeginID
            { get { return StartId; } }
        }
        #endregion
    
}
