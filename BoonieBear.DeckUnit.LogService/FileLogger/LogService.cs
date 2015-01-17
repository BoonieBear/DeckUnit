using System;

namespace BoonieBear.DeckUnit.LogService.FileLogger
{
    public class LogService
    {
        public static ILogService GetLogger(Type t)
        {
            return new FileLogService(t);
        }
    }
}
