using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace BoonieBear.DeckUnit.Utilities.JSON
{
    public class NodeLogic
    {
        
        #region Fields


        #endregion Fields

        #region Constructors

        public NodeLogic(string text, string data, string meaning,List<NodeLogic> children )
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
        public NodeLogic Father { get; set; }
        public List<NodeLogic> Children { get; set; }

        

        #endregion Properties
    }
}
