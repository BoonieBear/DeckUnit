using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoonieBear.DeckUnit.CommLib.Serial
{
    public class CCycleMem
    {        
		private int iSize;//T的字节数
        private int iWriteOffsetPos;//头指针的偏移位置，下一个要写数的起始位置。取值范围[0,n-1]
        private int iReadOffsetPos;//尾指针的偏移位置，当前可读数的起始位置。取值范围[0,n-1]
        private int iTotalLen;//缓冲区总长度iTotalLen＝n
        private int iDataLen;//缓冲区中的数据长度
        private int iEmptyLen;//缓冲区的空闲长度iEmptyLen＋iDataLen＝iTotalLen

        public byte[] buffer_test = new byte[2000];// 测试使用

        public CCycleMem()
        {
           //将头指针和尾指针的偏移位置置为0
	        iReadOffsetPos=0;
	        iWriteOffsetPos=0;

	
	        iTotalLen=2000;//置缓冲区总长
	        iSize=1;

	        iDataLen=0;//数据长度为0
	        iEmptyLen=iTotalLen;//空闲长度为总长
        }

//		~CCycleMem(){};

		public void Reset()//头尾指针归零，将缓冲区清空
        {
	        iReadOffsetPos=iWriteOffsetPos=0;
	        iDataLen=0;//数据长度为0
	        iEmptyLen=iTotalLen;//空闲长度为总长
        }
        ///尾指针移到头指针，将缓冲区清空
        public void Empty()
        {
	        iReadOffsetPos=iWriteOffsetPos;
	        iDataLen=0;//数据长度为0
	        iEmptyLen=iTotalLen;//空闲长度为总长
        }
        public bool isEmpty()//缓冲区是否为空：真为空，否为非空
        {
	        if((iReadOffsetPos-iWriteOffsetPos)==0)
		        return true;
	        else
		        return false;
        }
        public bool isFull()//缓冲区是否已满
        {
            if ((iReadOffsetPos - iWriteOffsetPos) == 1 || (iReadOffsetPos - iWriteOffsetPos) == -iTotalLen + 1)
                return true;
            else
                return false;
            // return (iDataLen==iTotalLen);
        }

        public int GetDataLen()//获取数据长度
        {
	        //int iDataLen;
	        iDataLen = iWriteOffsetPos-iReadOffsetPos;
	        if (iDataLen<0) {
		        iDataLen += iTotalLen;
	        }
	        return iDataLen;
        }
        public int GetEmptyLen()//获取空闲长度
        {
	        //int iEmptyLen;
	        iEmptyLen = iReadOffsetPos - iWriteOffsetPos-1;
	        if(iEmptyLen<0)		iEmptyLen += iTotalLen-1;
	
	        return iEmptyLen;
        }
        public int GetTotalLen()//缓存区总长
        {
            return iTotalLen;
        }

        public int WriteData(byte[] scr, int count)//向缓冲区中写入数据
        {
            //	ASSERT(scr);
            iEmptyLen = GetEmptyLen();
            if (count > iEmptyLen) return -1;//数据长度大于空闲长度，则返回错误
            int spare = iTotalLen - iWriteOffsetPos;//头指针到缓冲区尾的长度

            if (count < spare)	//数据能一次拷贝完毕
            {
                Buffer.BlockCopy(scr, 0, buffer_test, iWriteOffsetPos, count * iSize);
                iWriteOffsetPos = iWriteOffsetPos + count;//移动头指针
            }
            else//数据分批拷贝
            {
                int i = count - spare;
                Buffer.BlockCopy(scr, 0, buffer_test, iWriteOffsetPos, spare * iSize);//从头指针的位置拷贝到缓冲区尾
                Buffer.BlockCopy(scr, spare, buffer_test, 0, i * iSize);//从缓冲区头将剩余的数据拷贝完毕
                iWriteOffsetPos = i;//移动头指针

            }

            return count;

        }
        public int ReadData(byte[] dest, int count)//从缓冲区中读走数据
        {
            //	ASSERT(dest);

            //if (count>iDataLen)	return -1;
            iDataLen = GetDataLen();
            count = Math.Min(count, iDataLen);

            if (count == 0) return -1;

            int spare = iTotalLen - iReadOffsetPos;//尾指针到缓冲区尾的长度

            if (count < spare)	//数据能一次拷贝完毕
            {
                Buffer.BlockCopy(buffer_test, iReadOffsetPos, dest, 0, count * iSize);
                iReadOffsetPos = iReadOffsetPos + count;//移动尾指针
            }
            else//数据分批拷贝
            {
                int i = count - spare;
                Buffer.BlockCopy(buffer_test, iReadOffsetPos, dest, 0, spare * iSize);//从头指针的位置拷贝到缓冲区尾
                Buffer.BlockCopy(buffer_test, 0, dest, spare, i * iSize);//从缓冲区头将剩余的数据拷贝完毕
                iReadOffsetPos = i;//移动头指针

            }

            return count;

        }

    }
}
