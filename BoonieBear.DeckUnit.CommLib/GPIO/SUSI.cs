using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SUSI.Library
{
    public static class SUSI_Parameter
    {
        public static string PlatformName = "";
    }

    public struct SSCORE_RUNTIMER
    {
        public UInt32 dwOPFlag;
        public Int32 isRunning;             // if define BOOL in Struct in C, remember to define it as Int32 in C#
        public Int32 isAutorun;
        public UInt32 dwTimeContinual;
        public UInt32 dwTimeTotal;
    }

    static public class SUSI_DLL
    {
        [DllImport("SUSI")]
        public static extern int SusiDllGetLastError();
        [DllImport("SUSI")]
        public unsafe static extern void SusiDllGetVersion(UInt32* major, UInt32* minor);
        [DllImport("SUSI")]
        public static extern bool SusiDllInit();
        [DllImport("SUSI")]
        public static extern bool SusiDllUnInit();
    }

    static public class SUSI_Core
    {
        //-----------------------------------------------------------------------------
        //	CPU Information
        //-----------------------------------------------------------------------------
        // Vendor
        public const UInt32 INTEL = 1 << 0;
        public const UInt32 VIA = 1 << 1;
        public const UInt32 SIS = 1 << 2;
        public const UInt32 NVIDIA = 1 << 3;
        public const UInt32 AMD = 1 << 4;
        public const UInt32 RDC = 1 << 5;


        [DllImport("SUSI")]
        public unsafe static extern bool SusiCoreAccessBootCounter(UInt32 mode, UInt32 OPFlag, bool* enable, UInt32* value);
        [DllImport("SUSI")]
        public static extern bool SusiCoreAccessRunTimer(UInt32 mode, ref SSCORE_RUNTIMER refRunTimer);
        [DllImport("SUSI")]
        public static extern int SusiCoreAvailable();
        [DllImport("SUSI")]
        public unsafe static extern int SusiCoreGetBIOSVersion(char* BIOSVersion, UInt32* size);
        [DllImport("SUSI")]
        public unsafe static extern int SusiCoreGetPlatformName(char* PlatformName, UInt32* size);

        // Old (backward-compatible)
        [DllImport("SUSI")]
        public unsafe static extern int SusiGetPlatformName(char* PlatformName, byte size);
        [DllImport("SUSI")]
        public unsafe static extern int SusiGetBIOSVersion(char* BIOSVersion, byte size);


        [DllImport("SUSI")]
        public static extern bool SusiCoreReadIO(UInt32 addr, ref UInt32 value);
        [DllImport("SUSI")]
        public static extern bool SusiCoreWriteIO(UInt32 addr, UInt32 value);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiCoreReadULongIO(UInt32 addr, UInt32* value);
        [DllImport("SUSI")]
        public static extern bool SusiCoreWriteULongIO(UInt32 addr, UInt32 value);

        [DllImport("SUSI")]
        public static extern bool SusiCoreGetCpuMaxSpeed(ref UInt32 Value);
        [DllImport("SUSI")]
        public static extern bool SusiCoreGetCpuVendor(ref UInt32 Value);

        [DllImport("SUSI")]
        public static extern bool SusiCoreEnableBootfail();
        [DllImport("SUSI")]
        public static extern bool SusiCoreDisableBootfail();
        [DllImport("SUSI")]
        public static extern bool SusiCoreRefreshBootfail();
    }

    static public class SUSI_HWM
    {
        // Hardware Monitor Control
        [DllImport("SUSI")]
        public static extern int SusiHWMAvailable();
        [DllImport("SUSI")]
        public unsafe static extern bool SusiHWMGetFanSpeed(UInt16 fanType, UInt16* retval, UInt16* typeSupport);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiHWMGetTemperature(UInt16 tempType, float* retval, UInt16* typeSupport);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiHWMGetVoltage(UInt32 voltType, float* retval, UInt32* typeSupport);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiHWMSetFanSpeed(UInt16 fanType, byte setval, UInt16* typeSupport);

        // Temperature flag
        public enum TempList : int
        {
            TCPU = 0,   // CPU temperature
            TSYS,       // System temperature
            TAUX,       // 3'rd thermal dioad
            TCPU2,
            TCount      // Only for detect temperature count
        }

        // Fan speed flag
        public enum FanList : int
        {
            FCPU = 0,   // CPU fan speed
            FSYS,       // System fan speed
            F2ND,       // Other fan speed
            FCPU2,
            FAUX2,
            FCount      // Only for detect fan count
        }

        // Voltage flag
        public enum VoltList : int
        {
            VCORE = 0,
            V25,
            V33,
            V50,
            V120,
            V5SB,
            V3SB,
            VBAT,
            VN50,
            VN120,
            VTT,
            VCORE2,
            V105,
            V15,
            V18,
            V240,
            VCount      // Only for detect voltage count
        }
    }

    static public class SUSI_WDT
    {
        public const UInt32 WDTOUT0 = 0;
        public const UInt32 WDTOUT1 = 1;

        [DllImport("SUSI")]
        public static extern int SusiWDAvailable();
        [DllImport("SUSI")]
        public static extern bool SusiWDDisable();
        [DllImport("SUSI")]
        public static extern bool SusiWDDisableEx(UInt32 group_number);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiWDGetRange(UInt32* minimum, UInt32* maximum, UInt32* stepping);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiWDSetConfig(UInt32 delay, UInt32 timeout);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiWDSetConfigEx(UInt32 group_number, UInt32 delay, UInt32 timeout);
        [DllImport("SUSI")]
        public static extern bool SusiWDTrigger();
        [DllImport("SUSI")]
        public static extern bool SusiWDTriggerEx(UInt32 group_number);
    }

    static public class SUSI_GPIO
    {
        public enum ESIO_MASKFLAG
        {
            // Static mask
            ESIO_SMASK_PIN_FULL = 0x0001,
            ESIO_SMASK_CONFIGURABLE = 0x0002,
            // Dynamic mask
            ESIO_DMASK_DIRECTION = 0x0020,
        }

        // Common 
        [DllImport("SUSI")]
        public static extern int SusiIOAvailable();

        // Programmable
        [DllImport("SUSI", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern bool SusiIOCountEx(UInt32* inCount, UInt32* outCount);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiIOQueryMask(UInt32 flag, UInt32* Mask);
        [DllImport("SUSI", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern bool SusiIOReadEx(byte PinNum, UInt32* status);
        [DllImport("SUSI", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern bool SusiIOReadMultiEx(UInt32 TargetPinMask, UInt32* StatusMask);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiIOSetDirection(byte PinNum, byte IO, UInt32* PinDirMask);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiIOSetDirectionMulti(UInt32 TargetPinMask, UInt32* PinDirMask);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiIOWriteEx(byte PinNum, Int32 status);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiIOWriteMultiEx(UInt32 TargetPinMask, UInt32 StatusMask);

        // Not programmable (backward-compatible)
        [DllImport("SUSI")]
        public unsafe static extern bool SusiIOCount(UInt16* inCount, UInt16* outCount);
        [DllImport("SUSI")]
        public static extern bool SusiIOInitial(UInt32 statuses);
        [DllImport("SUSI", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern bool SusiIORead(byte pin, bool* status);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiIOReadMulti(UInt32 pins, UInt32* statuses);
        [DllImport("SUSI")]
        public static extern bool SusiIOWrite(byte pin, bool status);
        [DllImport("SUSI")]
        public static extern bool SusiIOWriteMulti(UInt32 pins, UInt32 statuses);
    }

    static public class SUSI_SMBus
    {
        [DllImport("SUSI")]
        public static extern int SusiSMBusAvailable();
        [DllImport("SUSI")]
        public unsafe static extern bool SusiSMBusReadBlock(byte SlaveAddress, byte RegisterOffset, byte* Result, byte* ByteCount);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiSMBusI2CReadBlock(byte SlaveAddress, byte RegisterOffset, byte* Result, byte* ByteCount);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiSMBusReadByte(byte SlaveAddress, byte RegisterOffset, byte* Result);
        [DllImport("SUSI")]
        public static extern bool SusiSMBusReadQuick(byte SlaveAddress);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiSMBusReadWord(byte SlaveAddress, byte RegisterOffset, UInt16* Result);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiSMBusReceiveByte(byte SlaveAddress, byte* Result);
        [DllImport("SUSI")]
        public static extern int SusiSMBusScanDevice(byte SlaveAddress_7);
        [DllImport("SUSI")]
        public static extern bool SusiSMBusSendByte(byte SlaveAddress, byte Result);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiSMBusWriteBlock(byte SlaveAddress, byte RegisterOffset, byte* Result, byte ByteCount);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiSMBusI2CWriteBlock(byte SlaveAddress, byte RegisterOffset, byte* Result, byte ByteCount);
        [DllImport("SUSI")]
        public static extern bool SusiSMBusWriteByte(byte SlaveAddress, byte RegisterOffset, byte Result);
        [DllImport("SUSI")]
        public static extern bool SusiSMBusWriteQuick(byte SlaveAddress);
        [DllImport("SUSI")]
        public static extern bool SusiSMBusWriteWord(byte SlaveAddress, byte RegisterOffset, UInt16 Result);
        [DllImport("SUSI")]
        public static extern bool SusiSMBusReset();
    }

    static public class SUSI_IIC
    {
        [DllImport("SUSI")]
        public static extern int SusiIICAvailable();
        [DllImport("SUSI")]
        public unsafe static extern bool SusiIICRead(UInt32 IICType, byte SlaveAddress, byte* ReadBuf, UInt32 ReadLen);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiIICWrite(UInt32 IICType, byte SlaveAddress, byte* WriteBuf, UInt32 WriteLen);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiIICWriteReadCombine(UInt32 IICType, byte SlaveAddress, byte* WriteBuf, UInt32 WriteLen, byte* ReadBuf, UInt32 ReadLen);

        // Old (backward-compatible)
        [DllImport("SUSI")]
        public unsafe static extern bool SusiIICReadByte(byte SlaveAddress, byte RegisterOffset, byte* Result);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiIICReadByteMulti(byte SlaveAddress, byte* ReadBuf, UInt32 ReadLen);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiIICWriteByte(byte SlaveAddress, byte RegisterOffset, byte Result);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiIICWriteByteMulti(byte SlaveAddress, byte* WriteBuf, UInt32 WriteLen);
    }

    static public class SUSI_VC
    {
        [DllImport("SUSI")]
        public static extern int SusiVCAvailable();
        [DllImport("SUSI")]
        public unsafe static extern bool SusiVCGetBright(ref byte brightness);
        [DllImport("SUSI")]
        public unsafe static extern bool SusiVCGetBrightRange(byte* minimum, byte* maximum, byte* stepping);
        [DllImport("SUSI")]
        public static extern bool SusiVCScreenOff();
        [DllImport("SUSI")]
        public static extern bool SusiVCScreenOn();
        [DllImport("SUSI")]
        public static extern bool SusiVCSetBright(byte brightness);
    }

    static public class SUSI_PowerSaving
    {
        [DllImport("kernel32")]
        public static extern IntPtr GetCurrentProcess();
        [DllImport("SUSI")]
        public static extern UInt32 SUSIPlusCpuSetOnDemandThrottling(IntPtr proc_handler, Byte cpu_index, Byte step);
        [DllImport("SUSI")]
        public unsafe static extern UInt32 SUSIPlusCpuGetOnDemandThrottling(IntPtr proc_handler, Byte cpu_index, ref byte step);

        [DllImport("SUSI")]
        public static extern UInt32 SUSIPlusCpuSetThrottling(Byte step);
        [DllImport("SUSI")]
        public unsafe static extern UInt32 SUSIPlusCpuGetThrottling(ref byte step);


        [DllImport("SUSI")]
        public static extern bool SusiPlusSpeedIsActive();
        [DllImport("SUSI")]
        public static extern int SusiPlusSpeedSetActive();
        [DllImport("SUSI")]
        public static extern int SusiPlusSpeedSetInactive();
        [DllImport("SUSI")]
        public static extern int SusiPlusSpeedWrite(byte ACPolicy, byte DCPolicy);
        [DllImport("SUSI")]
        public static extern int SusiPlusSpeedRead(ref byte ACPolicy, ref byte DCPolicy);

        [DllImport("SUSI")]
        public static extern float SusiPlusGetProcessorSpeed();

    }

    static public class SUSI_Hotkey
    {
        [DllImport("SUSI")]
        public unsafe static extern bool SusiTrekGetHotkey(byte* DI_data);
        [DllImport("SUSI")]
        public static extern int SusiTrekHotkeyAvailable();
    }
}
