using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BoonieBear.DeckUnit.Utilities.JSON
{
    public class NodeLogic
    {

        #region Fields


        #endregion Fields

        #region Constructors

        public NodeLogic(string text, string type, IEnumerable<NodeLogic> children)
        {
            Text = text;
            Type = type;
            Children = children;
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<NodeLogic> Children { get; set; }

        public string Text { get; set; }

        public string Type { get; set; }

        #endregion Properties
    }
}
