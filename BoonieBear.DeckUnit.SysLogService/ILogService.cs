using System;

namespace BoonieBear.DeckUnit.SysLogService
{
    /// <summary>
    /// 程序运行记录服务，用于记录各个子模块的运行状态和错误信息
    /// </summary>
    public interface ILogService
    {
        void Debug(object message);
        void Debug(object message, Exception exception);
        void DebugFormat(string format, params object[] args);
      
        void Error(object message);
        void Error(object message, Exception exception);
        void ErrorFormat(string format, params object[] args);
        
        void Fatal(object message);
        void Fatal(object message, Exception exception);
        void FatalFormat(string format, params object[] args);
        
        void Info(object message);
        void Info(object message, Exception exception);
        void InfoFormat(string format, params object[] args);
       
        void Warn(object message);
        void Warn(object message, Exception exception);
        void WarnFormat(string format, params object[] args);
    }
}
