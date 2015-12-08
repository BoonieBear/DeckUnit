using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using BoonieBear.DeckUnit.ACMP;
using BoonieBear.DeckUnit.Mov4500UI.Core;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using DevExpress.XtraCharts.Native;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;
using System.Windows.Media.Media3D;
using System.Net.NetworkInformation;
using System.Globalization;
namespace BoonieBear.DeckUnit.Mov4500UI.ViewModel
{
    public class LiveCaptureViewModel : ViewModelBase
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
        public float MovLong
        {
            get { return GetPropertyValue(() => MovLong); }
            set { SetPropertyValue(() => MovLong, value); }
        }
        public float MovLat
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

        public int HeadmainV
        {
            get { return GetPropertyValue(() => HeadmainV); }
            set { SetPropertyValue(() => HeadmainV, value); }
        }
        public int HeadmainI
        {
            get { return GetPropertyValue(() => HeadmainI); }
            set { SetPropertyValue(() => HeadmainI, value); }
        }
        public int Headmainconsume
        {
            get { return GetPropertyValue(() => Headmainconsume); }
            set { SetPropertyValue(() => Headmainconsume, value); }
        }
        public int HeadmainMaxTemp
        {
            get { return GetPropertyValue(() => HeadmainMaxTemp); }
            set { SetPropertyValue(() => HeadmainMaxTemp, value); }
        }
        public int HeadmainMaxExpand
        {
            get { return GetPropertyValue(() => HeadmainMaxExpand); }
            set { SetPropertyValue(() => HeadmainMaxExpand, value); }
        }
        public int TailmainV
        {
            get { return GetPropertyValue(() => TailmainV); }
            set { SetPropertyValue(() => TailmainV, value); }
        }
        public int TailmainI
        {
            get { return GetPropertyValue(() => TailmainI); }
            set { SetPropertyValue(() => TailmainI, value); }
        }
        public int Tailmainconsume
        {
            get { return GetPropertyValue(() => Tailmainconsume); }
            set { SetPropertyValue(() => Tailmainconsume, value); }
        }
        public int TailmainMaxTemp
        {
            get { return GetPropertyValue(() => TailmainMaxTemp); }
            set { SetPropertyValue(() => TailmainMaxTemp, value); }
        }
        public int TailmainMaxExpand
        {
            get { return GetPropertyValue(() => TailmainMaxExpand); }
            set { SetPropertyValue(() => TailmainMaxExpand, value); }
        }
        public int LeftsubV
        {
            get { return GetPropertyValue(() => LeftsubV); }
            set { SetPropertyValue(() => LeftsubV, value); }
        }
        public int LeftsubI
        {
            get { return GetPropertyValue(() => LeftsubI); }
            set { SetPropertyValue(() => LeftsubI, value); }
        }
        public int Leftsubconsume
        {
            get { return GetPropertyValue(() => Leftsubconsume); }
            set { SetPropertyValue(() => Leftsubconsume, value); }
        }
        public int LeftsubMaxTemp
        {
            get { return GetPropertyValue(() => LeftsubMaxTemp); }
            set { SetPropertyValue(() => LeftsubMaxTemp, value); }
        }
        public int LeftsubMaxExpand
        {
            get { return GetPropertyValue(() => LeftsubMaxExpand); }
            set { SetPropertyValue(() => LeftsubMaxExpand, value); }
        }
        public int RightsubV
        {
            get { return GetPropertyValue(() => RightsubV); }
            set { SetPropertyValue(() => RightsubV, value); }
        }
        public int RightsubI
        {
            get { return GetPropertyValue(() => RightsubI); }
            set { SetPropertyValue(() => RightsubI, value); }
        }
        public int Rightsubconsume
        {
            get { return GetPropertyValue(() => Rightsubconsume); }
            set { SetPropertyValue(() => Rightsubconsume, value); }
        }
        public int RightsubMaxTemp
        {
            get { return GetPropertyValue(() => RightsubMaxTemp); }
            set { SetPropertyValue(() => RightsubMaxTemp, value); }
        }
        public int RightsubMaxExpand
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
        public ObservableCollection<int> xVel
        {
            get { return GetPropertyValue(() => xVel); }
            set { SetPropertyValue(() => xVel, value); }
        }
        public ObservableCollection<int> yVel
        {
            get { return GetPropertyValue(() => yVel); }
            set { SetPropertyValue(() => yVel, value); }
        }
        public ObservableCollection<int> zVel
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

        }
        #endregion
    }
}
