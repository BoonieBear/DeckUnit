using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoonieBear.DeckUnit.Helps
{
    public class Utility
    {
        /// <summary>
        /// 返回数据的固定长度字符串，如2.34，固定长度为5，小数点位置在3，则返回00234，小数点位置在2，则返回02340
        /// </summary>
        static public string GetFormedString(string src, int dotindex, int length)
        {
            int SrcDotIndex = src.IndexOf('.');
            src = src.Replace(".", "");
            if (SrcDotIndex > dotindex)
            {
                throw (new Exception("GetFormedString输入格式错误！"));
            }
            int move = dotindex - SrcDotIndex;
            src = src.PadLeft(src.Length + move, '0');
            src = src.PadRight(length, '0');
            return src;

        }
    }
}
