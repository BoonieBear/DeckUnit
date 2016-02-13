using System;

namespace BoonieBear.DeckUnit.Device.USBL
{
    public class USBLParser
    {
        char[] m_sFullData ;//完整的一帧数据
        private int m_nDataLen = 1;
        public long Time;
        public  float ShipLat { get; private set; }
        public float ShipLng { get; private set; }
        public float MovLng { get; private set; }
        public float MovLat { get; private set; }
        public short X { get; private set; }
        public short Y { get; private set; }
        public ushort Z { get; private set; }
        public ushort Heading { get; private set; }
        public short Pitch { get; private set; }
        public short Roll { get; private set; }
        public UInt16 MovDepth { get; private set; }
        public Int16 ShipVelocity { get; private set; }

        public bool Parse(string data)
        {
            if (!IsCorrect(data))
            {
                return false;
            }
            else
            {
                switch (CheckDataType(data))
                {
                    case 1:
                        Time = GetTime();
                        float Long, Lat;
                        if (IsShipData(data))
                        {
                            GetThePost(data, out Long, out Lat);
                            ShipLat = Lat;
                            ShipLng = Long;
                        }
                        else
                        {
                            GetThePost(data, out Long, out Lat);
                            GetAUVDepth(data);
                            MovLat = Lat;
                            MovLng = Long;
                        }
                        break;
                    case 2:
                        if (!IsShipData(data))
                        {
                            GetRelativeData(data);
                        }
                        break;
                    case 3:
                        GetShipStance(data);
                        break;
                    default:
                        break;
                }
                return true;
            }

        }
        public bool IsCorrect(string data)
        {
            m_sFullData = data.ToCharArray();
            m_nDataLen = data.Length;
            int sum = m_sFullData[1];
            int sum1 = 0;
            string str;
            int i = 0;

            if (m_nDataLen < 8 || m_sFullData[m_nDataLen - 5] != '*')
            {
                m_nDataLen = 0;
                return false;
            }

            for (i = 0; i < 2; i++)
            {
                char ch = m_sFullData[m_nDataLen - 4 + i];
                if (('0' <= ch) & (ch <= '9'))
                {
                    sum1 = sum1 + (ch - 48) * (((1 - i) << 4) + i);
                }

                else
                {
                    if ((('a' <= ch) & (ch <= 'f')) || (('A' <= ch) & (ch <= 'F')))
                    {
                        int tmp = i;
                        ch = char.ToUpper(ch);
                        i = tmp;
                        sum1 = sum1 + (ch - 55) * (((1 - i) << 4) + i);
                    }
                    else
                    {
                        m_nDataLen = 0;

                        return false;
                    }
                }
            }

            for (int j = 2; j < m_nDataLen - 5; j++)//从$后开始校验
            {
                sum = sum ^ m_sFullData[j];
            }

            if (sum != sum1)
            {
                return false;
            }
            return true;
        }
        public int CheckDataType(string Data)
        {
            if (GetData(Data, 0, ',') == "$PTSAG")//PTSAG
            {
                return 1;
            }
            else if (GetData(Data, 0, ',') == "$PTSAX")
            {
                return 2;
            }
            else if (GetData(Data, 0, ',') == "PTSAZ")
            {
                return 3;
            }
            else
                return 0;
        }

        private string GetData(string data, int nindex, char split)
        {
            var str = data.Split(split);
            return str[nindex];
        }
        private long GetTime()
        {
            return DateTime.Now.ToFileTime();

        }
        private bool IsShipData(string Data)
        {
	        if(GetData(Data,6,',')=="0")
		        return true;
	        else
		        return false;

        }
        private void GetAUVDepth(string Data)
        {
            MovDepth = (UInt16)(float.Parse(GetData(Data, 12, ',')) * 65536.0 / 5000);
            if (MovDepth > 10000 * 65536 / 5000)
	        {
		        MovDepth= 0;
	        }
        }
        void GetThePost(string Data, out float Long, out float Lat)
        {
		        string str;

		        if (GetData(Data,8,',')=="N")//纬度
		        {
			        str = GetData(Data,7,',');
			        Lat=float.Parse(str.Substring(0,2))*60 + float.Parse(str.Substring(2,8));
			        if( Lat > 90.1*60)
			        {
				        Lat =0 ;
			        }
		        } 
		        else
		        {
			        str = GetData(Data,7,',');
			        Lat =0-float.Parse(str.Substring(0,2))*60 - float.Parse(str.Substring(2,8));
			        if( Lat < -90.1*60)
			        {
				        Lat =0 ;
			        }
		        }
		        if (GetData(Data,10,',')=="E")//经度
		        {
			        str = GetData(Data,9,',');
			        Long = float.Parse(str.Substring(0,3))*60 + float.Parse(str.Substring(3,8));
			        if(Long > 180.1*60)
			        {
				        Long = 0;
			        }
		        } 
		        else
		        {
			        str = GetData(Data,9,',');
			        Long = 0-float.Parse(str.Substring(0,3))*60 - float.Parse(str.Substring(3,8));
			        if( Long < -180.1*60)
			        {
				        Long = 0;
			        }
		        }
        }

        void GetRelativeData(string Data)
        {

            X = (short)(float.Parse(GetData(Data, 7, ',')) * 32768.0 / 15000);
            Y = (short)(float.Parse(GetData(Data, 8, ',')) * 32768.0 / 15000);
            Z = (ushort)(float.Parse(GetData(Data, 10, ',')) * 65536.0 / 5000);

        }

        void GetShipStance(string Data)
        {
            Heading = (ushort)(float.Parse(GetData(Data, 8, ',')) * 65536.0 / (2 * Math.PI));//弧度->角度
            Roll = (short)(float.Parse(GetData(Data, 9, ',')) * 65536.0 / (2 * Math.PI));//弧度->角度
            Pitch = (short)(float.Parse(GetData(Data, 10, ',')) * 65536.0 / (2 * Math.PI));//弧度->角度
        }

    }
}
