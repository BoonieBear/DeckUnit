namespace BoonieBear.DeckUnit.Resource
{

   public interface IPcResources 
    {
        double GetMemoryUsage();
        double GetDiskUsage(string diskName);
        double GetDiskSize(string diskName);
        double GetDiskFree(string diskName); 
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
