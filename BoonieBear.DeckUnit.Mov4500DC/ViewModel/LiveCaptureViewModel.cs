using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using BoonieBear.DeckUnit.ACMP;
using BoonieBear.DeckUnit.Mov4500UI.Core;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using DevExpress.Xpf.Ribbon.Customization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraRichEdit.Utils.NumberConverters;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.Utility.Schedular;
using TinyMetroWpfLibrary.ViewModel;
using System.Windows.Media.Media3D;
using System.Net.NetworkInformation;
using System.Globalization;
using TinyMetroWpfLibrary.EventAggregation;
using System.Collections;
using System.IO;
using System.Windows.Media.Imaging;
namespace BoonieBear.DeckUnit.Mov4500UI.ViewModel
{
    public class LiveCaptureViewModel : ViewModelBase, IHandleMessage<MovDataEvent>, IHandleMessage<USBLEvent>
    {
        private DispatcherTimer t;
        private NetworkInterface currentInterface = null;
        private NetworkInterface[] networkInterfaces = null;
        double bytesFormerReceived;
        double bytesFormerSent;

        public override void Initialize()
        {
            MovHeading = 45;
            ShipHeading = 135;
            XMTValue = 0.001F;
            XMTIndex = 0;
            LinkStatus = "未连接通信机";
            LinkBoxColor = new SolidColorBrush(Colors.Crimson);
            xVel = new ObservableCollection<sbyte>();
            yVel = new ObservableCollection<sbyte>();
            zVel = new ObservableCollection<sbyte>();
            AlarmList = new ObservableCollection<string>();
            xVel.CollectionChanged+=xVel_CollectionChanged;
            yVel.CollectionChanged +=yVel_CollectionChanged;
            zVel.CollectionChanged+=zVel_CollectionChanged;
            AlarmList.CollectionChanged +=AlarmList_CollectionChanged;
        }

        private void AlarmList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnPropertyChanged(()=>AlarmList);
        }

        private void zVel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnPropertyChanged(()=>zVel);
        }

        private void yVel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnPropertyChanged(()=>yVel);
        }

        private void xVel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnPropertyChanged(()=>xVel);
        }

        //将x,y,z参数转化为3D图上的相对坐标
        private void Transfromxyz(float x, float y,float z)
        {
            X = -x*2.5f;
            Y = y*2.5f;
            Z = -z*2.5f;
        }
        public override void InitializePage(object extraData)
        {
            BtnShow = (UnitCore.Instance.WorkMode == MonitorMode.SHIP) ? true : false;
            XDistance = 300;
            YDistance = 600;
            ZDistance = 400;
            Transfromxyz(XDistance, YDistance, ZDistance);
            if (t==null)
                t = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, Tick, Dispatcher.CurrentDispatcher);
            if (networkInterfaces==null)
                networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            if (networkInterfaces.Length == 0)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("没有找到可用网卡！",LogType.Both));
            }
            else
            {
                t.IsEnabled = true;
            }
        }

        private void Tick(object sender, EventArgs e)
        {
            if (UnitCore.Instance.NetCore.IsTCPWorking)
            {
                LinkStatus = "已连接通信机";
                LinkBoxColor = new SolidColorBrush(Colors.White);
            }
            else
            {
                LinkStatus = "未连接通信机";
                LinkBoxColor = new SolidColorBrush(Colors.Crimson);
            }
            currentInterface = networkInterfaces[0];
            IPv4InterfaceStatistics ipStats = currentInterface.GetIPv4Statistics();

            NumberFormatInfo numberFormat = NumberFormatInfo.CurrentInfo;

            double receiveByte = ipStats.BytesReceived / 1024;
            double SentBype = ipStats.BytesSent / 1024;
            double bytesReceived = receiveByte - bytesFormerReceived ;
            double bytesSent = SentBype - bytesFormerSent;
            if (bytesReceived < 1024)
                LinkRxSpeed = bytesReceived.ToString("N0", numberFormat) + " B/s";
            else if (bytesReceived < 1024*1024)
            {
                LinkRxSpeed = bytesReceived.ToString("N0", numberFormat) + " KB/s";
            }
            else
            {
                LinkRxSpeed = bytesReceived.ToString("N0", numberFormat) + " MB/s";
            }
            if (bytesSent < 1024)
                LinkTxSpeed = bytesSent.ToString("N0", numberFormat) + " B/s";
            else if (bytesSent < 1024 * 1024)
            {
                LinkTxSpeed = bytesSent.ToString("N0", numberFormat) + " KB/s";
            }
            else
            {
                LinkTxSpeed = bytesSent.ToString("N0", numberFormat) + " MB/s";
            }
            bytesFormerReceived = receiveByte;
            bytesFormerSent = SentBype;
        }

        //show btn or control based on the workmode 
        //if workmode == ship tnshow = true, or btnshow=false
        public bool BtnShow
        {
            get { return GetPropertyValue(() => BtnShow); }
            set { SetPropertyValue(() => BtnShow, value); }
        }

        #region data binding
        public int XMTIndex
        {
            get { return GetPropertyValue(() => XMTIndex); }
            set { SetPropertyValue(() => XMTIndex, value); }
        }
        
        #region status
        public string LinkRxSpeed
        {
            get { return GetPropertyValue(() => LinkRxSpeed); }
            set { SetPropertyValue(() => LinkRxSpeed, value); }
        }
        public string LinkTxSpeed
        {
            get { return GetPropertyValue(() => LinkTxSpeed); }
            set { SetPropertyValue(() => LinkTxSpeed, value); }
        }
        public string LinkStatus
        {
            get { return GetPropertyValue(() => LinkStatus); }
            set { SetPropertyValue(() => LinkStatus, value); }
        }

        public SolidColorBrush LinkBoxColor
        {
            get { return GetPropertyValue(() => LinkBoxColor); }
            set { SetPropertyValue(() => LinkBoxColor, value); }
        }
        #endregion

        #region position
        public string PosFromUwvTime
        {
            get { return GetPropertyValue(() => PosFromUwvTime); }
            set { SetPropertyValue(() => PosFromUwvTime, value); }
        }
        public string PosFromUSBLTime
        {
            get { return GetPropertyValue(() => PosFromUSBLTime); }
            set { SetPropertyValue(() => PosFromUSBLTime, value); }
        }
        //from usbl/gps/PlaneS1
        public string ShipLongUsbl
        {
            get { return GetPropertyValue(() => ShipLongUsbl); }
            set { SetPropertyValue(() => ShipLongUsbl, value); }
        }
        public string ShipLatUsbl
        {
            get { return GetPropertyValue(() => ShipLatUsbl); }
            set { SetPropertyValue(() => ShipLatUsbl, value); }
        }
        public string MovLongUsbl
        {
            get { return GetPropertyValue(() => MovLongUsbl); }
            set { SetPropertyValue(() => MovLongUsbl, value); }
        }
        public string MovLatUsbl
        {
            get { return GetPropertyValue(() => MovLatUsbl); }
            set { SetPropertyValue(() => MovLatUsbl, value); }
        }
        public float MovDepthUsbl
        {
            get { return GetPropertyValue(() => MovDepthUsbl); }
            set { SetPropertyValue(() => MovDepthUsbl, value); }
        }
        public float ShipHeading
        {
            get { return GetPropertyValue(() => ShipHeading); }
            set { SetPropertyValue(() => ShipHeading, value); }
        }

        public float ShipPitch
        {
            get { return GetPropertyValue(() => ShipPitch); }
            set { SetPropertyValue(() => ShipPitch, value); }
        }

        public float ShipRoll
        {
            get { return GetPropertyValue(() => ShipRoll); }
            set { SetPropertyValue(() => ShipRoll, value); }
        }

        public float ShipSpeed
        {
            get { return GetPropertyValue(() => ShipSpeed); }
            set { SetPropertyValue(() => ShipSpeed, value); }
        }
        //from mov
        public string MovLong
        {
            get { return GetPropertyValue(() => MovLong); }
            set { SetPropertyValue(() => MovLong, value); }
        }
        public string MovLat
        {
            get { return GetPropertyValue(() => MovLat); }
            set { SetPropertyValue(() => MovLat, value); }
        }
        public float MovDepth
        {
            get { return GetPropertyValue(() => MovDepth); }
            set { SetPropertyValue(() => MovDepth, value); }
        }
        public float MovHeading
        {
            get { return GetPropertyValue(() => MovHeading); }
            set { SetPropertyValue(() => MovHeading, value); }
        }

        public float MovPitch
        {
            get { return GetPropertyValue(() => MovPitch); }
            set { SetPropertyValue(() => MovPitch, value); }
        }

        public float MovRoll
        {
            get { return GetPropertyValue(() => MovRoll); }
            set { SetPropertyValue(() => MovRoll, value); }
        }

        public float MovHeight
        {
            get { return GetPropertyValue(() => MovHeight); }
            set { SetPropertyValue(() => MovHeight, value); }
        }
        
        //pos binding by map, can be calced by usbl or mov stats
        public float XDistance
        {
            get { return GetPropertyValue(() => XDistance); }
            set { SetPropertyValue(() => XDistance, value); }
        }
        public float YDistance
        {
            get { return GetPropertyValue(() => YDistance); }
            set { SetPropertyValue(() => YDistance, value); }
        }
        public float ZDistance
        {
            get { return GetPropertyValue(() => ZDistance); }
            set { SetPropertyValue(() => ZDistance, value); }
        }
        public float X
        {
            get { return GetPropertyValue(() => X); }
            set { SetPropertyValue(() => X, value); }
        }
        public float Y
        {
            get { return GetPropertyValue(() => Y); }
            set { SetPropertyValue(() => Y, value); }
        }
        public float Z
        {
            get { return GetPropertyValue(() => Z); }
            set { SetPropertyValue(() => Z, value); }
        }
        #endregion
        #region alarm

        public string AlarmTime
        {
            get { return GetPropertyValue(() => AlarmTime); }
            set { SetPropertyValue(() => AlarmTime, value); }
        }

        public float Leak
        {
            get { return GetPropertyValue(() => Leak); }
            set { SetPropertyValue(() => Leak, value); }
        }
        public float Cable
        {
            get { return GetPropertyValue(() => Cable); }
            set { SetPropertyValue(() => Cable, value); }
        }
        public float AlertTemp
        {
            get { return GetPropertyValue(() => AlertTemp); }
            set { SetPropertyValue(() => AlertTemp, value); }
        }
        public ObservableCollection<string> AlarmList
        {
            get { return GetPropertyValue(() => AlarmList); }
            set { SetPropertyValue(() => AlarmList, value); }
        }
        #endregion
        #region energy
        public string EnergyTime
        {
            get { return GetPropertyValue(() => EnergyTime); }
            set { SetPropertyValue(() => EnergyTime, value); }
        }

        public float HeadmainV
        {
            get { return GetPropertyValue(() => HeadmainV); }
            set { SetPropertyValue(() => HeadmainV, value); }
        }
        public float HeadmainI
        {
            get { return GetPropertyValue(() => HeadmainI); }
            set { SetPropertyValue(() => HeadmainI, value); }
        }
        public float Headmainconsume
        {
            get { return GetPropertyValue(() => Headmainconsume); }
            set { SetPropertyValue(() => Headmainconsume, value); }
        }
        public float HeadmainMaxTemp
        {
            get { return GetPropertyValue(() => HeadmainMaxTemp); }
            set { SetPropertyValue(() => HeadmainMaxTemp, value); }
        }
        public float HeadmainMaxExpand
        {
            get { return GetPropertyValue(() => HeadmainMaxExpand); }
            set { SetPropertyValue(() => HeadmainMaxExpand, value); }
        }
        public float TailmainV
        {
            get { return GetPropertyValue(() => TailmainV); }
            set { SetPropertyValue(() => TailmainV, value); }
        }
        public float TailmainI
        {
            get { return GetPropertyValue(() => TailmainI); }
            set { SetPropertyValue(() => TailmainI, value); }
        }
        public float Tailmainconsume
        {
            get { return GetPropertyValue(() => Tailmainconsume); }
            set { SetPropertyValue(() => Tailmainconsume, value); }
        }
        public float TailmainMaxTemp
        {
            get { return GetPropertyValue(() => TailmainMaxTemp); }
            set { SetPropertyValue(() => TailmainMaxTemp, value); }
        }
        public float TailmainMaxExpand
        {
            get { return GetPropertyValue(() => TailmainMaxExpand); }
            set { SetPropertyValue(() => TailmainMaxExpand, value); }
        }
        public float LeftsubV
        {
            get { return GetPropertyValue(() => LeftsubV); }
            set { SetPropertyValue(() => LeftsubV, value); }
        }
        public float LeftsubI
        {
            get { return GetPropertyValue(() => LeftsubI); }
            set { SetPropertyValue(() => LeftsubI, value); }
        }
        public float Leftsubconsume
        {
            get { return GetPropertyValue(() => Leftsubconsume); }
            set { SetPropertyValue(() => Leftsubconsume, value); }
        }
        public float LeftsubMaxTemp
        {
            get { return GetPropertyValue(() => LeftsubMaxTemp); }
            set { SetPropertyValue(() => LeftsubMaxTemp, value); }
        }
        public float LeftsubMaxExpand
        {
            get { return GetPropertyValue(() => LeftsubMaxExpand); }
            set { SetPropertyValue(() => LeftsubMaxExpand, value); }
        }
        public float RightsubV
        {
            get { return GetPropertyValue(() => RightsubV); }
            set { SetPropertyValue(() => RightsubV, value); }
        }
        public float RightsubI
        {
            get { return GetPropertyValue(() => RightsubI); }
            set { SetPropertyValue(() => RightsubI, value); }
        }
        public float Rightsubconsume
        {
            get { return GetPropertyValue(() => Rightsubconsume); }
            set { SetPropertyValue(() => Rightsubconsume, value); }
        }
        public float RightsubMaxTemp
        {
            get { return GetPropertyValue(() => RightsubMaxTemp); }
            set { SetPropertyValue(() => RightsubMaxTemp, value); }
        }
        public float RightsubMaxExpand
        {
            get { return GetPropertyValue(() => RightsubMaxExpand); }
            set { SetPropertyValue(() => RightsubMaxExpand, value); }
        }
        #endregion
        #region adcp
        public string ADCPTime
        {
            get { return GetPropertyValue(() => ADCPTime); }
            set { SetPropertyValue(() => ADCPTime, value); }
        }
        public ObservableCollection<sbyte> xVel
        {
            get { return GetPropertyValue(() => xVel); }
            set { SetPropertyValue(() => xVel, value); }
        }
        public ObservableCollection<sbyte> yVel
        {
            get { return GetPropertyValue(() => yVel); }
            set { SetPropertyValue(() => yVel, value); }
        }
        public ObservableCollection<sbyte> zVel
        {
            get { return GetPropertyValue(() => zVel); }
            set { SetPropertyValue(() => zVel, value); }
        }
        #endregion
        #region life supply
        public string LifeTime
        {
            get { return GetPropertyValue(() => LifeTime); }
            set { SetPropertyValue(() => LifeTime, value); }
        }

        public float Oxygen
        {
            get { return GetPropertyValue(() => Oxygen); }
            set { SetPropertyValue(() => Oxygen, value); }
        }
        public float Co2
        {
            get { return GetPropertyValue(() => Co2); }
            set { SetPropertyValue(() => Co2, value); }
        }
        public float Pressure
        {
            get { return GetPropertyValue(() => Pressure); }
            set { SetPropertyValue(() => Pressure, value); }
        }
        public float Temperature
        {
            get { return GetPropertyValue(() => Temperature); }
            set { SetPropertyValue(() => Temperature, value); }
        }
        public float Humidity
        {
            get { return GetPropertyValue(() => Humidity); }
            set { SetPropertyValue(() => Humidity, value); }
        }
        #endregion
        #region ctd
        public string CTDTime
        {
            get { return GetPropertyValue(() => CTDTime); }
            set { SetPropertyValue(() => CTDTime, value); }
        }
        public float CTDWaterTemp
        {
            get { return GetPropertyValue(() => CTDWaterTemp); }
            set { SetPropertyValue(() => CTDWaterTemp, value); }
        }
        public float CTDVartlevel
        {
            get { return GetPropertyValue(() => CTDVartlevel); }
            set { SetPropertyValue(() => CTDVartlevel, value); }
        }

        public float CTDWatercond
        {
            get { return GetPropertyValue(() => CTDWatercond); }
            set { SetPropertyValue(() => CTDWatercond, value); }
        }
        public float CTDSoundvec
        {
            get { return GetPropertyValue(() => CTDSoundvec); }
            set { SetPropertyValue(() => CTDSoundvec, value); }
        }
        #endregion
        #region bp
        public string BpTime
        {
            get { return GetPropertyValue(() => BpTime); }
            set { SetPropertyValue(() => BpTime, value); }
        }
        public float BpFrontUp
        {
            get { return GetPropertyValue(() => BpFrontUp); }
            set { SetPropertyValue(() => BpFrontUp, value); }
        }
        public float BpLeft
        {
            get { return GetPropertyValue(() => BpLeft); }
            set { SetPropertyValue(() => BpLeft, value); }
        }
        public float BpRight
        {
            get { return GetPropertyValue(() => BpRight); }
            set { SetPropertyValue(() => BpRight, value); }
        }
        public float BpFront
        {
            get { return GetPropertyValue(() => BpFront); }
            set { SetPropertyValue(() => BpFront, value); }
        }
        public float BpFrontDown
        {
            get { return GetPropertyValue(() => BpFrontDown); }
            set { SetPropertyValue(() => BpFrontDown, value); }
        }
        public float BpBottom
        {
            get { return GetPropertyValue(() => BpBottom); }
            set { SetPropertyValue(() => BpBottom, value); }
        }
        public float BpBottomBack
        {
            get { return GetPropertyValue(() => BpBottomBack); }
            set { SetPropertyValue(() => BpBottomBack, value); }
        }

        #endregion
        #region bsss
        public string BsssTime
        {
            get { return GetPropertyValue(() => BsssTime); }
            set { SetPropertyValue(() => BsssTime, value); }
        }
        public int BsssHeight
        {
            get { return GetPropertyValue(() => BsssHeight); }
            set { SetPropertyValue(() => BsssHeight, value); }
        }
        #endregion
        
        public float XMTValue
        {
            get { return GetPropertyValue(() => XMTValue); }
            set { SetPropertyValue(() => XMTValue, value); }
        }
        #endregion
        #region Command

        public ICommand AddFHCMD
        {
            get { return GetPropertyValue(() => AddFHCMD); }
            set { SetPropertyValue(() => AddFHCMD, value); }
        }
        public void CanExecuteAddFHCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteAddFHCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            
        }
        public ICommand AddImgCMD
        {
            get { return GetPropertyValue(() => AddImgCMD); }
            set { SetPropertyValue(() => AddImgCMD, value); }
        }
        public void CanExecuteAddImgCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteAddImgCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            //tbd
        }
        #endregion

        public void Handle(MovDataEvent message)
        {
            string chartstring = null;
            System.Windows.Controls.Image ImgContainer = null;
            if (message.Data.ContainsKey(MovDataType.ADCP))
            {
                var adcp = message.Data[MovDataType.ADCP] as Adcpdata;
                if (adcp != null)
                {
                    ADCPTime = DateTime.FromFileTime(adcp.Itime).ToShortTimeString();
                    xVel.Clear();
                    xVel.AddRange(adcp.FloorX);
                    yVel.Clear();
                    yVel.AddRange(adcp.FloorY);
                    zVel.Clear();
                    zVel.AddRange(adcp.FloorZ);
                }
            }
            if (message.Data.ContainsKey(MovDataType.ALERT))
            {
                var alt = message.Data[MovDataType.ALERT] as Alertdata;
                if (alt != null)
                {
                    AlarmTime = DateTime.FromFileTime(alt.Ltime).ToShortTimeString();
                    Leak = alt.Leak;
                    Cable = alt.Cable;
                    AlertTemp = alt.Temperature;
                    var lst = BitConverter.GetBytes(alt.Alert);
                    BitArray ba = new BitArray(lst);
                    int i = 0;
                    foreach (bool a in ba)
                    {
                        if (a == true)
                        {
                            AlarmList.Add(Alarm.Name[i]);
                        }
                        i++;
                    }
                    //广播
                    UnitCore.Instance.MovTraceService.Save("ACOUSTICTOSAIL", alt.Pack());
                    byte[] posBytes = new byte[22];
                    posBytes[0] = 0x05;
                    posBytes[1] = 0x10;
                    Buffer.BlockCopy(alt.Pack(), 0, posBytes, 2, 20);
                    UnitCore.Instance.NetCore.BroadCast(posBytes);
                }
            }
            if (message.Data.ContainsKey(MovDataType.ALLPOST))
            {

                var allpos = message.Data[MovDataType.ALLPOST] as Sysposition;
                if (allpos != null)
                {
                    PosFromUSBLTime = DateTime.FromFileTime(allpos.Ltime).ToShortTimeString();
                    ShipLongUsbl = allpos.ShipLong;
                    ShipLatUsbl = allpos.ShipLat;
                    ShipHeading = allpos.Shipheading;
                    ShipPitch = allpos.Shippitch;
                    ShipRoll = allpos.Shiproll;
                    ShipSpeed = allpos.Shipvel;
                    MovLongUsbl = allpos.SubLong;
                    MovLatUsbl = allpos.SubLat;
                    MovDepthUsbl = allpos.Subdepth;
                    XDistance = allpos.RelateX;
                    YDistance = allpos.RelateY;
                    ZDistance = allpos.RelateZ;
                    Transfromxyz(XDistance, YDistance, ZDistance);
                    //广播
                    UnitCore.Instance.MovTraceService.Save("ACOUSTICTOSAIL", allpos.Pack());
                    byte[] posBytes = new byte[42];
                    posBytes[0] = 0x01;
                    posBytes[1] = 0x20;
                    Buffer.BlockCopy(allpos.Pack(),0,posBytes,2,40);
                    UnitCore.Instance.NetCore.BroadCast(posBytes);
                }
            }
            if (message.Data.ContainsKey(MovDataType.BP))
            {
                var bp = message.Data[MovDataType.BP] as Bpdata;
                if (bp != null)
                {
                    BpTime = DateTime.FromFileTime(bp.Itime).ToShortTimeString();
                    BpBottom = bp.Down;
                    BpBottomBack = bp.Behinddown;
                    BpFront = bp.Front;
                    BpFrontDown = bp.Frontdown;
                    BpFrontUp = bp.Frontup;
                    BpLeft = bp.Left;
                    BpRight = bp.Right;
                }
            }
            if (message.Data.ContainsKey(MovDataType.BSSS))
            {
                var bsss = message.Data[MovDataType.BSSS] as Bsssdata;
                if (bsss != null)
                {
                    BsssTime = DateTime.FromFileTime(bsss.Itime).ToShortTimeString();
                    BsssHeight = bsss.Height;
                }
            }
            if (message.Data.ContainsKey(MovDataType.CTD))
            {
                var ctd = message.Data[MovDataType.CTD] as Ctddata;
                if (ctd != null)
                {
                    CTDTime = DateTime.FromFileTime(ctd.Ltime).ToShortTimeString();
                    CTDSoundvec = ctd.Soundvec;
                    CTDVartlevel = ctd.Vartlevel;
                    CTDWaterTemp = ctd.Watertemp;
                    CTDWatercond = ctd.Watercond;
                    //广播
                    UnitCore.Instance.MovTraceService.Save("ACOUSTICTOSAIL", ctd.Pack());
                    byte[] posBytes = new byte[18];
                    posBytes[0] = 0x02;
                    posBytes[1] = 0x10;
                    Buffer.BlockCopy(ctd.Pack(), 0, posBytes, 2, 16);
                    UnitCore.Instance.NetCore.BroadCast(posBytes);
                }
            }
            if (message.Data.ContainsKey(MovDataType.ENERGY))
            {
                var eng = message.Data[MovDataType.ENERGY] as Energysys;
                if (eng != null)
                {
                    EnergyTime = DateTime.FromFileTime(eng.Ltime).ToShortTimeString();
                    HeadmainV = eng.HeadmainV;
                    HeadmainI = eng.HeadmainI;
                    Headmainconsume = eng.Headmainconsume;
                    HeadmainMaxTemp = eng.HeadmainMaxTemp;
                    HeadmainMaxExpand = eng.HeadmainMaxExpand;
                    TailmainV = eng.TailmainV;
                    TailmainI = eng.TailmainI;
                    Tailmainconsume = eng.Tailmainconsume;
                    TailmainMaxTemp = eng.TailmainMaxTemp;
                    TailmainMaxExpand = eng.TailmainMaxTemp;
                    LeftsubV = eng.LeftsubV;
                    LeftsubI = eng.LeftsubI;
                    Leftsubconsume = eng.Leftsubconsume;
                    LeftsubMaxTemp = eng.LeftsubMaxTemp;
                    LeftsubMaxExpand = eng.LeftsubMaxExpand;
                    RightsubV = eng.RightsubV;
                    RightsubI = eng.RightsubI;
                    Rightsubconsume = eng.Rightsubconsume;
                    RightsubMaxTemp = eng.RightsubMaxTemp;
                    RightsubMaxExpand = eng.RightsubMaxExpand;
                    //广播
                    UnitCore.Instance.MovTraceService.Save("ACOUSTICTOSAIL", eng.Pack());
                    byte[] posBytes = new byte[36];
                    posBytes[0] = 0x04;
                    posBytes[1] = 0x10;
                    Buffer.BlockCopy(eng.Pack(), 0, posBytes, 2, 34);
                    UnitCore.Instance.NetCore.BroadCast(posBytes);
                }
            }
            if (message.Data.ContainsKey(MovDataType.IMAGE))
            {
                byte[] bytes = message.Data[MovDataType.IMAGE] as byte[];
                if (bytes != null)
                {
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = ms;
                        image.EndInit();
                        ImgContainer = new System.Windows.Controls.Image();
                        ImgContainer.Source = image;
                    }
                }

            }
            if (message.Data.ContainsKey(MovDataType.LIFESUPPLY))
            {
                var life = message.Data[MovDataType.LIFESUPPLY] as Lifesupply;
                if (life != null)
                {
                    LifeTime = DateTime.FromFileTime(life.Ltime).ToShortTimeString();
                    Oxygen = life.Oxygen;
                    Co2 = life.Co2;
                    Pressure = life.Pressure;
                    Temperature = life.Temperature;
                    Humidity = life.Humidity;
                    //广播
                    UnitCore.Instance.MovTraceService.Save("ACOUSTICTOSAIL", life.Pack());
                    byte[] posBytes = new byte[16];
                    posBytes[0] = 0x03;
                    posBytes[1] = 0x10;
                    Buffer.BlockCopy(life.Pack(), 0, posBytes, 2, 14);
                    UnitCore.Instance.NetCore.BroadCast(posBytes);
                }
            }
            if (message.Data.ContainsKey(MovDataType.SUBPOST))
            {

                var subpos = message.Data[MovDataType.SUBPOST] as Subposition;
                if (subpos != null)
                {
                    PosFromUwvTime = DateTime.FromFileTime(subpos.Ltime).ToShortTimeString();
                    MovDepth = subpos.Subdepth;
                    MovLat = subpos.SubLat;
                    MovLong = subpos.SubLong;
                    MovHeading = subpos.Subheading;
                    MovHeight = subpos.Subheight;
                    MovPitch = subpos.Subpitch;
                    MovRoll = subpos.Subroll;
                    //广播
                    UnitCore.Instance.MovTraceService.Save("ACOUSTICTOSAIL", subpos.Pack());
                    byte[] posBytes = new byte[28];
                    posBytes[0] = 0x01;
                    posBytes[1] = 0x10;
                    Buffer.BlockCopy(subpos.Pack(), 0, posBytes, 2, 26);
                    UnitCore.Instance.NetCore.BroadCast(posBytes);
                }
            }
            if (message.Data.ContainsKey(MovDataType.WORD))
            {
                    var word = message.Data[MovDataType.WORD] as string;
                    if (word != null)
                    {
                        if(UnitCore.Instance.WorkMode== MonitorMode.SHIP)
                            UnitCore.Instance.MovTraceService.Save("Chart","（潜器）"+word);
                        else
                        {
                            UnitCore.Instance.MovTraceService.Save("Chart","（母船）"+word);
                        }
                        chartstring = word;
                    }
                    
            }
            UnitCore.Instance.LiveHandle(message.Type, chartstring,ImgContainer);
        }

        public void Handle(USBLEvent message)
        {
            var allpos = message.Position as Sysposition;
            if (allpos != null)
            {
                PosFromUSBLTime = DateTime.FromFileTime(allpos.Ltime).ToShortTimeString();
                ShipLongUsbl = allpos.ShipLong;
                ShipLatUsbl = allpos.ShipLat;
                ShipHeading = allpos.Shipheading;
                ShipPitch = allpos.Shippitch;
                ShipRoll = allpos.Shiproll;
                ShipSpeed = allpos.Shipvel;
                MovLongUsbl = allpos.SubLong;
                MovLatUsbl = allpos.SubLat;
                MovDepthUsbl = allpos.Subdepth;
                XDistance = allpos.RelateX;
                YDistance = allpos.RelateY;
                ZDistance = allpos.RelateZ;
                Transfromxyz(XDistance, YDistance, ZDistance);
            }
        }
    }
}
