using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoonieBear.DeckUnit.CommLib.Serial
{
    class CCheckBP: CCheck
    {
        public CCheckBP()
        {
            m_bFlag = false;
            m_nDataLen = 0;
        }

        public bool IsCorrect()//数据帧校验是否正确
        {
	        uint sum=0;
	        int sum1=0;
	        string str;
	        byte[] checksum=new byte[4];

	        if(m_nDataLen<19 || m_sFullData[m_nDataLen-7]!=0x2A)//'*'
	        {
		        m_nDataLen=0;
		        m_bFlag=false;
		        //没有找到*
		        return false;
	        }

	        for(int j=0;j<m_nDataLen-6;j++)
	        {
		        sum=sum+m_sFullData[j];
	        }
	        byte check =0x30;//'0';
	        checksum[0] = check;
	        check = (byte)(sum>>8);
	       // checksum[1] = ((check)<10)?(check+'0'):(check-10+'A');
            checksum[1] =(byte) ((check<0x0a)?(check+0x30):(check-0x0a+0x41));
	        check = (byte)sum;
	        check = (byte)(check/16);
	        checksum[2] = (byte) ((check<0x0a)?(check+0x30):(check-0x0a+0x41));
	        check = (byte)sum;
	        check = (byte)(check&0x0f);
            checksum[3] = (byte)((check < 0x0a) ? (check + 0x30) : (check - 0x0a + 0x41));
	        for(int i=0;i<4;i++)
	        {
		        if(checksum[i]!=m_sFullData[m_nDataLen-6+i])
		        {
			        m_nDataLen=0;
			        m_bFlag=false;
			        return false;
		        }
	        }
	        return true;
        }

       
        
    }
}
