using System.Collections.Generic;
using AngusChangyiMods.Logger;

namespace AngusChangyiMods.Core.Test
{
    public class MockLogger : ILogger
    {
        public List<LogInfo> Logs = new();

        public void Log(LogInfo logInfo)
        {
            Logs.Add(logInfo);
        }
    }
}