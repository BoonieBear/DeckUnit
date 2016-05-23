using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoonieBear.DeckUnit.CommLib.Serial
{
    public class CCheck
    {
        public bool m_bFlag ;//是否有‘$'
        public bool eFlag;//是否有0x0D
	    public CCycleMem DataMem=new CCycleMem();//存放原数据
        public byte[] m_sFullData = new byte[200];//完整的一帧数据
        public int m_nDataLen;//帧的数据长度


        public CCheck()
        {
	        m_bFlag=false;
            eFlag = false;
	        m_nDataLen=0;
        }


//------------获取完整的一帧数据－－－－－－－－－－－－－
        public bool IsFull()
        {

	        int DataLen=DataMem.GetDataLen();//得到缓冲区中总的数据长度
	        int count=0;

	        //AfxMessageBox("CCheck::GetFull()");

	        for(int i=0;i<DataLen;i++)
	        {
		        byte[] data=new byte[1];

		        if(m_nDataLen>199) //数据超长放弃,将m_sFullData复位（199bits）
		        {
			        m_nDataLen=0;
			        m_bFlag=false;
			        break;
		        }

		        DataMem.ReadData((byte[])data,1);
		        switch(data[0])
		        {
                case 0x24://找到帧头'$'
			        /*if(!m_bFlag)*/
			        m_bFlag=true;//有帧头
			        m_nDataLen=0;//m_sFullData复位
			        m_sFullData[m_nDataLen]=data[0];//写入数据
			        m_nDataLen++;
			        //AfxMessageBox("找到帧头");
			        break;
		        case 0x0D://'\r'
			        if(!m_bFlag) break;//没有帧头不写入
			        eFlag=true;//找到回车
			        count=i;
			        m_sFullData[m_nDataLen]=data[0];
			        m_nDataLen++;
			        break;
		        case 0x0A://'\n'
			        if(!m_bFlag) break;//没有帧头不写入

			        if(!eFlag) //没有回车标志
			        {
				        m_sFullData[m_nDataLen]=data[0];
				        m_nDataLen++;
				        break;
			        }
			        else//有回车标志
			        {
				        //if(i-count==1)//是上个字符为回车
                        if (m_sFullData[m_nDataLen-1]==0x0D)//有可能0x0A与0x0D是分开进check的
				        {
					        eFlag=false;
					        m_sFullData[m_nDataLen]=data[0];
					        m_nDataLen++;
					        m_bFlag=false;
					        return true;//已经取到完整的一帧
				        }
				        eFlag=false;
				        m_sFullData[m_nDataLen]=data[0];
				        m_nDataLen++;
				        break;
			        }
		        default:
			        if(!m_bFlag) break;//没有帧头不写入
			        m_sFullData[m_nDataLen]=data[0];
			        m_nDataLen++;
                    break;
		        }
	        }
	        return false;//没有取到完整的一帧数据

        }
    
//-------------检验数据和-----------------------------------------
        public bool IsCorrect()
        {
	        int sum=m_sFullData[1];
	        int sum1=0;
	        string str;


	        if(m_nDataLen<8 || m_sFullData[m_nDataLen-5]!='*')
	        {
		        m_nDataLen=0;
		        m_bFlag=false;
		        //AfxMessageBox("没有找到*");
		        return false;
	        }

	        for(int i=0;i<2;i++)
	        {
                char ch = System.Convert.ToChar(m_sFullData[m_nDataLen - 4 + i]);
		        if(('0'<=ch)&(ch<='9'))	 
		        {
			        sum1=sum1+(ch-48)*(((1-i)<<4)+i);
				
			        //str.Format("初步校验和是%d",sum1);
			        //AfxMessageBox(str);
		        }

		        else
		        {
			        if((('a'<=ch)&(ch<='f'))||(('A'<=ch)&(ch<='F')))
			        {
                        ch = char.ToUpper(ch);
			            sum1=sum1+(ch-55)*(((1-i)<<4)+i);
			        }

			        else
			        {
			            //MessageBox.Show("数据格式不对");
			            m_nDataLen=0;
                        m_bFlag = false;
			            return false;
			        }
		        }
	        }

	        /*str.Format("校验和是%d",sum1);
	        AfxMessageBox(str);*/

	        for(int j=2;j<m_nDataLen-5;j++)
	        {
		        sum=sum^m_sFullData[j];
	        }
		
	        /*str.Format("计算和是%d",sum);
	        AfxMessageBox(str);*/

	        if(sum!=sum1)
	        {
		        m_nDataLen=0;
		        m_bFlag=false;
                return false;
	        }
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
	        dwLen=(uint)m_nDataLen;
            Buffer.BlockCopy(m_sFullData, 0, dest, 0, m_nDataLen * sizeof(byte));
	        return ;
        }
//--------------数据帧长度--------------------------------------------------

        public int GetLen()
        {
	        return m_nDataLen;
        }
    
    }

}
