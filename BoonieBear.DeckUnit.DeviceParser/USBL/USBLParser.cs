using System;

namespace BoonieBear.DeckUnit.Device.USBL
{
    public class USBLParser
    {
        public static double Lat { get; private set; }
        public static double Lng { get; private set; }
        public static bool bFlag = false;
        public static string Errormessage { get; private set; }

        public static bool Parse(byte[] data, int index)
        {
            //do the parse job
            try
            {
                bFlag = true;
            }
            catch (Exception e)
            {

                Errormessage += e.Message;
                bFlag = false;
            }

            return bFlag;

        }
    }
}
