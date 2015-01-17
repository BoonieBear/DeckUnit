using System;

namespace BoonieBear.DeckUnit.Utilities.FileLogger
{
    public class LogService
    {
        public static ILogService GetLogger(Type t)
        {
            return new FileLogService(t);
        }
    }
}
