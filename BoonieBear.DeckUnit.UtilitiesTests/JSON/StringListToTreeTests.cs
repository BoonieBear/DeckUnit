using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using BoonieBear.DeckUnit.CommLib.Protocol;
using NUnit.Framework;
using BoonieBear.DeckUnit.Utilities.JSON;
using Newtonsoft.Json;

namespace BoonieBear.DeckUnit.UtilitiesTests.JSON
{
    [TestFixture]
    public class StringListToTreeTests
    {
        
        [Test]
        public void TransListToNodeWriteLineicTest()
        {
            var strlStringses = new List<string[]>
            {
                new[] {"0", "块数", "3", ""},
                new[] {"0", "块数1", "33", ""},
                new[] {"1", "块数2", "33", ""},
                new[] {"1", "块数3", "53", ""},
                new[] {"1", "块数4", "43", ""},
                new[] {"2", "块数5", "3", ""},
                new[] {"3", "块数6", "32", ""},
                new[] {"4", "块数7", "23", ""},
                new[] {"3", "块数8", "3", ""},
                new[] {"4", "块数9", "13", ""},
                new[] {"2", "块数", "31", ""}
            };

            var jsonstr = StringListToTree.LstToJson(strlStringses);
            Debug.WriteLine(jsonstr);
            var node = JsonConvert.DeserializeObject<NodeWriteLineic>(jsonstr);  
            
             Assert.IsNotNull(node);
        }

        
    }
}
