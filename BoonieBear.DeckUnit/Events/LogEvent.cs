using System;

namespace BoonieBear.DeckUnit.Events
{
    public  enum LogType
    {
        OnlyLog,
        OnlyInfo,
        Both
    }
    public class LogEvent
    {
        private string message;
        private LogType type;
        
        public LogEvent(string message, LogType type)
        {
            this.message = message;
            this.type = type;
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        public LogType Type
        {
            get { return type; }
            set { type = value; }
        }
    }

    public class ErrorEvent
    {
        private string message;
        private LogType type;
        private Exception ex;
        public ErrorEvent(Exception ex, LogType type)
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