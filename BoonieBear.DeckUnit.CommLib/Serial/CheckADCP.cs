using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoonieBear.DeckUnit.CommLib.Serial
{
    public class CheckADCP
    {
        public bool m_bFlag1;//是否有第一个0x7F
	    public bool m_bFlag2;//是否有二个0x7F
	    public CCycleMem DataMem=new CCycleMem();//存放原数据
	    public byte[] m_nFullData = new byte[4096];//完整的一帧数据
	    public short m_nReadLen;//已经读到帧的数据长度
	    public short  m_nFrameLen;//数据帧的实际长度
	    public short  m_nCheckSum;//校验和

        public CheckADCP()
        {
            m_bFlag1 = false;
            m_bFlag2 = false;
	        m_nReadLen = 0;
	        m_nFrameLen = 0;
	        m_nCheckSum = 0;
        }


//------------获取完整的一帧数据－－－－－－－－－－－－－
       /* public bool IsFull()
        {

	        int DataLen=DataMem.GetDataLen();//得到缓冲区中总的数据长度

	        bool eFlag=false;
	        int count=0;
            byte[] data = new byte[1];
	        //AfxMessageBox("CCheck::GetFull()");

	        for(int i=0;i<DataLen;i++)
	        {
                if (m_nReadLen > 1024) //如果数据非法，可能导致找不到ID
		        {
                    m_nReadLen = 0;  //放弃已经进入缓冲区的数据
                    m_nFrameLen = 0;
                    count = 0;
                    m_nCheckSum = 0;  //重新计算校验和
                    m_bFlag1 = false;
                    m_bFlag2 = false;
		        }
                DataMem.ReadData(data, 1);

                if(m_bFlag2 == false ) //还没有找到ID
		        {
			        if( data[0]== 0x7F)
			        {
				        if(!m_bFlag1 && !m_bFlag2)  //找到第一个7F
				        {
					        m_bFlag1 = true;
					        count = i;
					        m_nCheckSum += data[0];
					        m_nFullData[m_nReadLen++] = data[0];
				        }
				        else if(m_bFlag1 && !m_bFlag2 && i == count+1)  //找到第二个7F
				        {
					        m_bFlag1 = false;
					        m_bFlag2 = true;
					        m_nCheckSum += data[0];
					        m_nFullData[m_nReadLen++] = data[0];
				        }
				        else if(m_bFlag1 && !m_bFlag2 && i != count+1)  //找到一个7F但与上一个7F不是连在一起的。
				        {
					        m_nReadLen = 0;  //放弃已经进入缓冲区的数据
					        m_nFrameLen = 0;
					        count = i;
					        m_nCheckSum = data[0];  //重新计算校验和
					        m_nFullData[m_nReadLen++] = data[0];
				        }
			        }
		        }
		        else //已找到ID
		        {
			        m_nFullData[m_nReadLen++] = data[0];
			        if(m_nReadLen == 4)  //从数据中读出祯长，不包括ID和校验和。
			        {
                        m_nFrameLen = BitConverter.ToInt16(m_nFullData,2);
			        }
			        if((m_nReadLen <= (m_nFrameLen)) || (m_nFrameLen == 0))
			        {
                        m_nCheckSum =(short)( m_nCheckSum + Convert.ToInt16(data[0]));
			        }
			        else if((m_nReadLen == (m_nFrameLen+2)) && (m_nFrameLen != 0))
			        {
				        short checksum;
                        checksum = BitConverter.ToInt16(m_nFullData, m_nFrameLen);
				        if(m_nCheckSum == checksum)
				        {
					        m_bFlag1 = false;
					        m_bFlag2 = false;
					        m_nReadLen = 0;
					        //m_nFrameLen = 0;
					        m_nCheckSum = 0;						
					        return  true;
				        }
				        else{
					        m_bFlag1 = false;
					        m_bFlag2 = false;
					        m_nReadLen = 0;
					        m_nFrameLen = 0;
					        m_nCheckSum = 0;
                            return false;
				        }						
			        }
		        }//if(m_bFlag2)

	        }

	        return false;//没有取到完整的一帧数据

        }*/

        public bool IsFull()
        {
            int DataLen=DataMem.GetDataLen();//得到缓冲区中总的数据长度

            byte[] data = new byte[DataLen];

            DataMem.ReadData(data, DataLen);
            Buffer.BlockCopy(data, 0, m_nFullData, m_nReadLen, DataLen);
            m_nReadLen = (short)(m_nReadLen+DataLen);
            if (m_nReadLen > 1024 || m_nReadLen==0) //如果数据非法，可能导致找不到ID
		    {
               m_nReadLen = 0;  //放弃已经进入缓冲区的数据
               return false;
		    }
            if (m_nReadLen>=454&&m_nFullData[m_nReadLen - 2] == '\r' && m_nFullData[m_nReadLen - 1] == '\n')
            {
                if (m_nFullData[m_nReadLen-58]==0x57&&m_nFullData[m_nReadLen-57]==0x44)
                {
                    //等收到B和W两组再解析
                    m_nFrameLen = m_nReadLen;
                    m_nReadLen = 0;
                    return true;
                }
                    
                else
                    return false;
            }
            else
            {
                return false;
            }

        }
    
//-------------检验数据和-----------------------------------------
        public bool IsCorrect()
        {
	        return true;
        }
//-------------将数据放入循环缓存区-----------------------------------------
        public int WriteData(byte[] buf,uint dwLen)
        {
	        return DataMem.WriteData(buf,(int)dwLen);//将接收到的数据放入循环缓冲区中
        }
//----------------------------------------------------------------

        public void GetFullData(byte[]dest,ref uint dwLen)
        {
            dwLen =(uint) m_nFrameLen;
            Buffer.BlockCopy(m_nFullData, 0, dest, 0, (m_nFrameLen) * sizeof(byte));
	        return ;
        }
//--------------数据帧长度--------------------------------------------------

        public int GetLen()
        {
            return (m_nFrameLen + 2);
        }

    }
}
