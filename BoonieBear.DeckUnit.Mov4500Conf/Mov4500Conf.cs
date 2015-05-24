using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoonieBear.DeckUnit.Mov4500Conf
{
    public class MovConf
    {
        private readonly static object SyncObject = new object();
        private static MovConf _movConf;

        //配置文件
        private string xmldoc = "BasicConf.xml";//const
        public static MovConf GetInstance()
        {
            lock (SyncObject)
            {
                return _movConf ?? (_movConf = new MovConf());
            }
        }

        protected MovConf()
        {
            string MyExecPath = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            xmldoc = MyExecPath + "\\" + xmldoc;

        }
    }
}
