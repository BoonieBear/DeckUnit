using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyMetroWpfLibrary.Utility;
using System.Globalization;
namespace BoonieBear.DeckUnit.ACNP
{
    /// <summary>
    /// 构建MSP特殊命令
    /// </summary>
    public class MSPHexBuilder
    {
        public static byte[] Pack255()
        {

        }

        public static byte[] Pack254()
        {

        }
        public static byte[] Pack253()
        {

        }

        public static byte[] Pack252(DateTime systimTime)
        {
            string strtime = systimTime.Year.ToString("0000", CultureInfo.InvariantCulture) + systimTime.Month.ToString("00", CultureInfo.InvariantCulture) + systimTime.Day.ToString("00", CultureInfo.InvariantCulture) + systimTime.Hour.ToString("00", CultureInfo.InvariantCulture)
                    + systimTime.Minute.ToString("00", CultureInfo.InvariantCulture) + systimTime.Second.ToString("00", CultureInfo.InvariantCulture);
            return ACNProtocol.CommPackage(252, StringHexConverter.ConvertHexToChar(strtime));
        }

        public static byte[] Pack251(DateTime sleepTime)
        {
            string strtime = sleepTime.Year.ToString("0000", CultureInfo.InvariantCulture) + sleepTime.Month.ToString("00", CultureInfo.InvariantCulture) + sleepTime.Day.ToString("00", CultureInfo.InvariantCulture) + sleepTime.Hour.ToString("00", CultureInfo.InvariantCulture)
                    + sleepTime.Minute.ToString("00", CultureInfo.InvariantCulture) + sleepTime.Second.ToString("00", CultureInfo.InvariantCulture);
            return ACNProtocol.CommPackage(251, StringHexConverter.ConvertHexToChar(strtime));
        }
        public static byte[] Pack250(bool bDebug)
        {
            byte[] cmd = new byte[1];
            cmd[0] = 0x00;
            if (bDebug)
                cmd[0] = 0x01;
            return ACNProtocol.CommPackage(250, cmd);
        }
        public static byte[] Pack249()
        {
            byte[] cmd = new byte[1];
            cmd[0] = 0xF9;//空命令
            return ACNProtocol.CommPackage(249, cmd);
        }
        public static byte[] Pack248(int com2waketime, int com3waketime)
        {
            string strtime = com2waketime.ToString("0000000000") + com3waketime.ToString("0000000000");
            return ACNProtocol.CommPackage(248, StringHexConverter.ConvertHexToChar(strtime));
        }
        public static byte[] Pack247()
        {
            byte[] cmd = new byte[1];
            cmd[0] = 0xF7;//空命令
            return ACNProtocol.CommPackage(247, cmd);
        }
        public static byte[] Pack246(bool bDog)
        {
            byte[] cmd = new byte[1];
            cmd[0] = 0x00;
            if (bDog)
                cmd[0] = 0x01;
            return ACNProtocol.CommPackage(246, cmd);
        }
        public static byte[] Pack245()
        {
            byte[] cmd = new byte[1];
            cmd[0] = 0xF5;//空命令
            return ACNProtocol.CommPackage(245, cmd);
        }
        public static byte[] Pack244()
        {
            byte[] cmd = new byte[1];
            cmd[0] = 0xF4;//空命令
            return ACNProtocol.CommPackage(244, cmd);
        }
        public static byte[] Pack243()
        {
            byte[] cmd = new byte[1];
            cmd[0] = 0xF3;//空命令
            return ACNProtocol.CommPackage(243, cmd);
        }
        public static byte[] Pack242()
        {
            byte[] cmd = new byte[1];
            cmd[0] = 0xF2;//空命令
            return ACNProtocol.CommPackage(242, cmd);
        }
        

        public static byte[] Pack20()
        {
            byte[] cmd = new byte[1];
            cmd[0] = 20;//空命令
            return ACNProtocol.CommPackage(20, cmd);
        }

        public static byte[] Pack19()
        {
            byte[] cmd = new byte[1];
            cmd[0] = 19;//空命令
            return ACNProtocol.CommPackage(19, cmd);
        }
        public static byte[] Pack18(bool bDebug)
        {
            byte[] cmd = new byte[1];
            cmd[0] = 00;
            if (bDebug)
                cmd[0] = 0x01;
            
            return ACNProtocol.CommPackage(18, cmd);
        }

        public static byte[] Pack17()
        {
            byte[] cmd = new byte[1];
            cmd[0] = 17;//空命令
            return ACNProtocol.CommPackage(17, cmd);
        }
        public static byte[] Pack16(bool bWork)
        {
            byte[] cmd = new byte[1];
            cmd[0] = 00;
            if (bWork)
                cmd[0] = 0x01;
            
            return ACNProtocol.CommPackage(16, cmd);
        }
    }
}
