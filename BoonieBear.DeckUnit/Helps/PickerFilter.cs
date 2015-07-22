using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyMetroWpfLibrary.FrameControls.Picker;
namespace BoonieBear.DeckUnit.Helps
{
    public class FilterItem : IItemFilter
    {
        /// <summary>
        /// Initializes a new instance of a FilterItem class
        /// </summary>
        /// <param name="text"></param>
        public FilterItem(string text)
        {
            Text = text;
            num = int.Parse(text);
        }

        public int num {get; private set; }
        /// <summary>
        /// Gets the Text value
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Filter the resources for the list picker
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool IsValueAccepted(string filter)
        {
            return filter == null || Text.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
