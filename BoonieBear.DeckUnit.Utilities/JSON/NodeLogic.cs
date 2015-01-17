using System.Collections.Generic;

namespace BoonieBear.DeckUnit.Utilities.JSON
{
    public class NodeWriteLineic
    {
        
        #region Fields


        #endregion Fields

        #region Constructors

        public NodeWriteLineic(string text, string data, string meaning,List<NodeWriteLineic> children )
        {
            Text = text;
            Data = data;
            Description = meaning;
            Children = children;
        }
        

        #endregion Constructors

        #region Properties
        public string Text { get; set; }

        public string Data { get; set; }

        public string Description { get; set; }
        public NodeWriteLineic Father { get; set; }
        public List<NodeWriteLineic> Children { get; set; }

        

        #endregion Properties
    }
}
