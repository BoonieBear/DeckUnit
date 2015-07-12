using System;

namespace BoonieBear.DeckUnit.Events
{
    public  enum LogType
    {
        Info,
        Warning,
        Error
    }
    public class LogEvent
    {
        private string message;
        private Exception ex;
        private LogType type;

        public LogEvent(Exception ex, LogType type)
        {
            this.Message = ex.Message;
            this.Ex = ex;
            this.Type = type;
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public Exception Ex
        {
            get { return ex; }
            set { ex = value; }
        }

        public LogType Type
        {
            get { return type; }
            set { type = value; }
        }
    }
}