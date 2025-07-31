
using System.Collections.Generic;

namespace AngusChangyiMods.Logger
{
    public class HistoryLogger : ILogger
    {
        private ILogger inner;
        public List<LogInfo> logs = new List<LogInfo>();
        
        public HistoryLogger() { }
        public HistoryLogger(ILogger inner)
        {
            this.inner = inner;
        }
        public void Log(LogInfo logInfo)
        {
            logs.Add(logInfo);
            inner?.Log(logInfo);
        }
    }
}