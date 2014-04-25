using System;

namespace BoonieBear.DeckUnit.Events
{
    public  enum LogType
    {
        Info,
        Error
    }
    public class LogEvent
    {
        private string message;
        private Exception ex;
        private LogType type;

        public LogEvent(string message, Exception ex, LogType type)
        {
            this.message = message;
            this.ex = ex;
            this.type = type;
        }
    }
}