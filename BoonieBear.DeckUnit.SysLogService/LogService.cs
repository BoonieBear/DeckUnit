using System;

namespace BoonieBear.DeckUnit.SysLogService
{
    public class LogService
    {
        public static ILogService GetLogger(Type t)
        {
            return new FileLogService(t);
        }
    }
}
