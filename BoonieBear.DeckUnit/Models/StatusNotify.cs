using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BoonieBear.DeckUnit.Models
{
    public enum NotifyLevel
    {
        Info,
        Warning,
        Error

    }
    public class StatusNotify
    {
        public string Source { get; set; }
        public string Msg { get; set; }
        public NotifyLevel Level { get; set; }
        public StatusNotify(string source, string msg, NotifyLevel level)
        {
            Source = source;
            Msg = msg;
            Level = level;
        }
    }
}
