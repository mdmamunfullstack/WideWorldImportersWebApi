using Contracts;
using NLog;

namespace LoggerService
{
    public class LoggerManager : ILoggerManager
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public void LogDebug(string message) { LoggerManager._logger.Debug(message); }

        public void LogError(string message) { LoggerManager._logger.Error(message); }

        public void LogInfo(string message) { LoggerManager._logger.Info(message); }

        public void LogWarn(string message) { LoggerManager._logger.Warn(message); }
    }
}