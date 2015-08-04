using BoonieBear.DeckUnit.Controls.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using BoonieBear.DeckUnit.JsonUtils;
namespace BoonieBear.DeckUnit.Models
{
    public class DataTreeModel : ITreeModel
    {
        private static NodeWriteLineic datatree = null;
        public DataTreeModel(NodeWriteLineic nodetree)
        {
            if(nodetree!=null)
                datatree = nodetree;
            else
            {
                throw new ArgumentNullException();
            }
        }
        public IEnumerable GetChildren(object parent)
        {
            var node = parent as NodeWriteLineic;
            if (parent == null)
            {
                yield return datatree;

            }
            else if (node != null)
            {
                foreach (var subnode in node.Children)
                {

                    if (subnode != null)
                        yield return subnode;
                }
                
                
            }
        }

        public bool HasChildren(object parent)
        {
            var node = parent as NodeWriteLineic;
            if (node != null)
            {
                return (node.Children != null);
            }
            return false;
        }
    }
    
}
