using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BoonieBear.DeckUnit.Utilities.JSON;
using Newtonsoft.Json;

namespace BoonieBear.DeckUnit.UtilitiesTests.JSON
{
    [TestClass()]
    public class StringListToTreeTests
    {
        [TestMethod()]
        public void TransListToNodeLogicTest()
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
            var nodetree = StringListToTree.TransListToNodeLogic(strlStringses);
            var newtree = StringListToTree.RemoveFatherPointer(nodetree);
            var jsonstr = JsonConvert.SerializeObject(newtree);
            var node = JsonConvert.DeserializeObject<NodeLogic>(jsonstr);  
             Assert.IsNotNull(node);
        }

        
    }
}
