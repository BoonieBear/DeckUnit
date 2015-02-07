using System;
using System.IO;
using log4net;

namespace BoonieBear.DeckUnit.SysLogService
{
    public sealed class FileLogService : ILogService
    {
        readonly ILog _logger;

        static FileLogService()
        {
            // Gets directory path of the calling application
            // RelativeSearchPath is null if the executing assembly i.e. calling assembly is a
            // stand alone exe file (Console, WinForm, etc). 
            // RelativeSearchPath is not null if the calling assembly is a web hosted application i.e. a web site
            var log4NetConfigDirectory = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;

            var log4NetConfigFilePath = Path.Combine(log4NetConfigDirectory, @"config\log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(log4NetConfigFilePath));

        }

        public FileLogService(Type logClass)
        {
            _logger = LogManager.GetLogger(logClass);
        }

        
        public void Debug(object message)
        {
            _logger.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            _logger.Debug(message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            _logger.DebugFormat(format, args);
        }

        public void Info(object message)
        {
            _logger.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            _logger.Info(message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _logger.InfoFormat(format, args);
        }

        public void Warn(object message)
        {
            _logger.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            _logger.Warn(message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _logger.WarnFormat(format, args);
        }

        public void Error(object message)
        {
            _logger.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            _logger.Error(message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _logger.ErrorFormat(format, args);
        }

        public void Fatal(object message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            _logger.Fatal(message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            _logger.FatalFormat(format, args);
        }

    }
}
