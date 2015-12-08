using System;

namespace BoonieBear.DeckUnit.Device.USBL
{
    public class USBLParser
    {
        public  double ShipLat { get; private set; }
        public  double ShipLng { get; private set; }
        public double MovLng { get; private set; }
        public double MovLat { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
        public float Heading { get; private set; }
        public float Pitch { get; private set; }
        public float Roll { get; private set; }
        public bool bShipPos { get; private set; }
        public bool bMovPos { get; private set; }

        public static string Errormessage { get; private set; }

        public static bool Parse(byte[] data, int index)
        {
            bool bParseOk = true;
            //do the parse job
            try
            {

            }
            catch (Exception e)
            {

                Errormessage = e.Message;
                bParseOk = false;
            }

            return bParseOk;
        }

        private static string GetData(string data, int nindex, char split)
        {
            var str = data.Split(split);
            return str[nindex];
        }
        private DateTime GetTime(string data)
        {
            return DateTime.Today;

        }
    }
}
