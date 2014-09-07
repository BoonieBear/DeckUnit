using System;
using System.Collections;
using System.Collections.Generic;

namespace BoonieBear.DeckUnit.Utilities
{
    internal class HashCopy
    {
        /// <summary>
        ///     拷贝hash表内容，string类型
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public static void CopyHashTableString(Hashtable src, Hashtable dst)
        {
            dst.Clear();
            foreach (var obj in src.Keys)
            {
                var nodename = (string) obj;
                var lst = (string) src[nodename];

                dst.Add(nodename, lst);
            }
        }

        /// <summary>
        ///     拷贝hash表内容，List<string>类型
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public static void CopyHashTableStringList(Hashtable src, Hashtable dst)
        {
            dst.Clear();
            foreach (var obj in src.Keys)
            {
                var nodename = (string) obj;
                var lst = (List<string>) src[nodename];
                var newlst = new List<string>(lst.Capacity);
                newlst.AddRange(lst);
                dst.Add(nodename, newlst);
            }
        }

        /// <summary>
        ///     拷贝hash表内容，List<string[]>类型
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public static void CopyHashTableStringListArray(Hashtable src, Hashtable dst)
        {
            dst.Clear();
            foreach (var obj in src.Keys)
            {
                var nodename = (string) obj;
                var lst = (List<string[]>) src[nodename];
                var newlst = new List<string[]>(lst.Capacity);
                foreach (var str in lst)
                {
                    var newstr = new string[str.Length];
                    Array.Copy(str, newstr, str.Length);
                    newlst.Add(newstr);
                }
                dst.Add(nodename, newlst);
            }
        }
    }
}