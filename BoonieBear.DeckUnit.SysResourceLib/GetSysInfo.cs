using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Management;
namespace BoonieBear.DeckUnit.SysResourceLib
{
    public class GetSysInfo : IPcResources
    {

        #region PC资源接口实现

        public  double GetMemoryUsage()
        {
            var w = new WMIInfo(WMIPath.Win32_OperatingSystem);
            double availableBytes = 0;
            double totalBytes = 0;
            for (var i = 0; i < w.Count; i++)
            {
                totalBytes += Math.Round(Int64.Parse(w[i, "TotalVisibleMemorySize"].ToString()) / 1024.0, 1);
                availableBytes += Math.Round(Int64.Parse(w[i, "FreePhysicalMemory"].ToString()) / 1024.0, 1);
            }

            return availableBytes*100/totalBytes;
        }

        public  double GetDiskUsage()
        {
            var w = new WMIInfo(WMIPath.Win32_LogicalDisk);
            double availableBytes = 0;
            double totalBytes = 0;
            for (var i = 0; i < w.Count; i++)
            {
                if (Convert.ToInt32(w[i, "DriveType"]) != Convert.ToInt32(DriveType.Fixed)) continue;
                totalBytes += Math.Round(Int64.Parse(w[i, "Size"].ToString()) / 1024.0, 1);
                availableBytes += Math.Round(Int64.Parse(w[i, "FreeSpace"].ToString()) / 1024.0, 1);
            }
            return 100-availableBytes * 100 / totalBytes;
        }

        public  string GetMacAddress()
        {
            var w = new WMIInfo(WMIPath.Win32_NetworkAdapterConfiguration);
            string addr = "00-00-00-00-00-00";
            for (int i = 0; i < w.Count; i ++)  
            {  
                if ((bool)w[i, "IPEnabled"])
                {
                    addr = w[i, "MACAddress"].ToString();
                    break;
                }  
            }
            return addr;
        }

        public  string GetIpAddress()
        {
            var w = new WMIInfo(WMIPath.Win32_NetworkAdapterConfiguration);
            string addr = "192.168.0.1";
            for (int i = 0; i < w.Count; i++)
            {
                if ((bool)w[i, "IPEnabled"])
                {
                    addr = ((string[])w[i, "IpAddress"])[0];
                    break;
                }
            }
            return addr;
        }

        #endregion

        #region WMI方式资源查询辅助

        /// <summary>
        /// WMI信息封装类
        /// </summary>
        public sealed class WMIInfo
        {
            private ArrayList mocs;
            private StringDictionary names; //用来存储属性名，键的处理方式是不区分大小写的；将键用于字符串字典之前会将其转换成小写。

            /// <summary>  
            /// 构造函数  
            /// </summary>  
            /// <param name="path"></param>  
            public WMIInfo(string path)
            {
                names = new StringDictionary();
                mocs = new ArrayList();

                try
                {
                    var cimobject = new ManagementClass(path);
                    var moc = cimobject.GetInstances();

                    var ok = false;
                    foreach (ManagementObject mo in moc)
                    {
                        var o = new Hashtable();
                        mocs.Add(o);
                        foreach (PropertyData p in mo.Properties)
                        {
                            o.Add(p.Name, p.Value);
                            if (!ok)
                            {
                                names.Add(p.Name, p.Name);
                            }
                        }
                        ok = true;
                        mo.Dispose();
                    }
                    moc.Dispose();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

            /// <summary>  
            /// 构造函数  
            /// </summary>  
            /// <param name="path"></param>  
            public WMIInfo(WMIPath path)
                : this(path.ToString())
            {
            }

            /// <summary>  
            /// 信息集合数量
            /// </summary>  
            public int Count
            {
                get { return mocs.Count; }
            }

            /// <summary>  
            /// 获取指定属性值，注意某些结果可能是数组。  
            /// </summary>  
            public object this[int index, string propertyName]
            {
                get
                {
                    try
                    {
                        string trueName = names[propertyName.Trim()]; //以此可不区分大小写获得正确的属性名称。  
                        var h = (Hashtable) mocs[index];
                        return h[trueName];
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            /// <summary>  
            /// 返回所有属性名称。  
            /// </summary>        
            public string[] PropertyNames(int index)
            {
                try
                {
                    var h = (Hashtable) mocs[index];
                    var result = new string[h.Keys.Count];

                    h.Keys.CopyTo(result, 0);

                    Array.Sort(result);
                    return result;
                }
                catch
                {
                    return null;
                }
            }
        }


        #region WMIPath

        public enum WMIPath
        {
            // 硬件  
            Win32_Processor, //CPU 处理器  
            Win32_PhysicalMemory, //物理内存条  
            Win32_Keyboard, //键盘  
            Win32_PointingDevice, //点输入设备，包括鼠标。  
            Win32_FloppyDrive, //软盘驱动器  
            Win32_DiskDrive, //硬盘驱动器  
            Win32_CDROMDrive, //光盘驱动器  
            Win32_BaseBoard, //主板  
            Win32_BIOS, //BIOS 芯片  
            Win32_ParallelPort, //并口  
            Win32_SerialPort, //串口  
            Win32_SerialPortConfiguration, //串口配置  
            Win32_SoundDevice, //多媒体设置，一般指声卡。  
            Win32_SystemSlot, //主板插槽 (ISA & PCI & AGP)  
            Win32_USBController, //USB 控制器  
            Win32_NetworkAdapter, //网络适配器  
            Win32_NetworkAdapterConfiguration, //网络适配器设置  
            Win32_Printer, //打印机
            Win32_PrinterConfiguration, //打印机设置  
            Win32_PrintJob, //打印机任务
            Win32_TCPIPPrinterPort, //打印机端口  
            Win32_POTSModem, //MODEM
            Win32_POTSModemToSerialPort, //MODEM 端口  
            Win32_DesktopMonitor, //显示器  
            Win32_DisplayConfiguration, //显卡  
            Win32_DisplayControllerConfiguration, //显卡设置  
            Win32_VideoController, //显卡细节。
            Win32_VideoSettings, //显卡支持的显示模式。  

            // 操作系统  
            Win32_TimeZone, //时区  
            Win32_SystemDriver, //驱动程序  
            Win32_DiskPartition, //磁盘分区  
            Win32_LogicalDisk, //逻辑磁盘  
            Win32_WriteLineicalDiskToPartition, //逻辑磁盘所在分区及始末位置。  
            Win32_WriteLineicalMemoryConfiguration, //逻辑内存配置  
            Win32_PageFile, //系统页文件信息  
            Win32_PageFileSetting, //页文件设置  
            Win32_BootConfiguration, //系统启动配置  
            Win32_ComputerSystem, //计算机信息简要  
            Win32_OperatingSystem, //操作系统信息  
            Win32_StartupCommand, //系统自动启动程序  
            Win32_Service, //系统安装的服务  
            Win32_Group, //系统管理组  
            Win32_GroupUser, //系统组帐号  
            Win32_UserAccount, //用户帐号  
            Win32_Process, //系统进程  
            Win32_Thread, //系统线程  
            Win32_Share, //共享  
            Win32_NetworkClient, //已安装的网络客户端  
            Win32_NetworkProtocol, //已安装的网络协议  
        }

        #endregion

        #endregion

        
    }

    /// <summary>
    /// 使用通信网协议获取通信机资源信息类，暂时没有可用的资源
    /// </summary>
    public class GetACNMspInfo : IMspResources
    {
        public double GetMspVoltage()
        {
            throw new NotImplementedException();
        }
    }
}
