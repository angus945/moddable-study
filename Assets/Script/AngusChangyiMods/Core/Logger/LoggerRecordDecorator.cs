
using System.Collections.Generic;

namespace AngusChangyiMods.Core
{
    public class LoggerRecordDecorator : AngusChangyiMods.Core.ILogger
    {
        private ILogger inner;
        public List<LogInfo> logs = new List<LogInfo>();
        
        public LoggerRecordDecorator(ILogger inner)
        {
            this.inner = inner;
        }
        public void Log(LogInfo logInfo)
        {
            logs.Add(logInfo);
            inner.Log(logInfo);
        }
        public void Log(string message, string tag = null)
        {
            logs.Add(new LogInfo(LogLevel.Info, message, tag));
            inner.Log(message, tag);
        }
        public void LogError(string message, string tag = null)
        {   
            logs.Add(new LogInfo(LogLevel.Error, message, tag));
            inner.LogError(message, tag);
        }
        public void LogWarning(string message, string tag = null)
        {
            logs.Add(new LogInfo(LogLevel.Warning, message, tag));
            inner.LogWarning(message, tag);
        }
    }
}