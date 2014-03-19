using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace BoonieBear.DeckUnit.Utilities.JSON
{
    public class StringListToTree
    {
        /// <summary>
        /// 将list<string>类型的解析数据转为nodelogic类，以便做json序列化
        /// </summary>
        /// <param name="strList">格式：(string level, string typename, string datastring, string description)</param>
        /// <returns></returns>
        public static NodeLogic TransListToNodeLogic(List<string[]> strList)
        {
            var node = new NodeLogic("信源数据包", null, null, null) {Father = null};
            var lastNode = node;
            var i = -1;
            foreach (var stringse in strList)
            {
                var j = int.Parse(stringse[0]);
                
                if (j > i)
                {
                    var tempnode = new NodeLogic(stringse[1], stringse[2], stringse[3], null);
                    if (lastNode.Children==null)
                        lastNode.Children = new List<NodeLogic> {tempnode};
                    else
                    {
                        lastNode.Children.Add(tempnode);
                    }
                    tempnode.Father = lastNode;
                    lastNode = tempnode;
                }
                else
                {
                    for (var tempj=j; tempj < i; tempj++)
                    {
                        lastNode = lastNode.Father;
                    }
                    var tempnode = new NodeLogic(stringse[1], stringse[2], stringse[3], null);
                    lastNode.Father.Children.Add(tempnode);
                    tempnode.Father = lastNode.Father;
                    lastNode = tempnode;
                }
                i = j;
            }
            return node;
        }

        /// <summary>
        /// 删除Father指针，以便json序列化时不会循环引用
        /// </summary>
        /// <param name="nodeLogic"></param>
        public static NodeLogic RemoveFatherPointer(NodeLogic nodeLogic)
        {
           
            if (nodeLogic.Children != null)
            {
                nodeLogic.Children.ForEach(logic => RemoveFatherPointer(logic));
            }

            nodeLogic.Father = null;
            return nodeLogic;
        }

        public static string LstToJson(List<string[]> strList)
        {
            var n =TransListToNodeLogic(strList);
            var newnode = RemoveFatherPointer(n);
            var json =  JsonConvert.SerializeObject(newnode);
            var s =json.Replace("\"Father\":null,", "");
            return s;
        }
        
    }

  
}
