using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Net.Configuration;
using System.Text;

namespace BoonieBear.DeckUnit.SysResourceLib
{

   public interface IPcResources 
    {
        double GetMemoryUsage();
        double GetDiskUsage();
        string GetMacAddress();
        string GetIpAddress();
    }

   public interface IMspResources
   {
       double GetMspVoltage();
   }


    public class GetSystemInfo
    {
        public static IPcResources CreateResourcesProbe()
        {
            IPcResources ir = new GetSysInfo();
            return ir;
        }
        public static IMspResources GreatemsMspResourcesProbe()
        {
            IMspResources ir = new GetACNMspInfo();
            return ir;
        }
    }

    
}
