namespace AngusChangyiMods.Core
{
    public class NullLogger : ILogger
    {
        public void Log(LogInfo logInfo) { }
        public void Log(string message, string tag = null) { /* Do nothing */ }
        public void LogWarning(string message, string tag = null) { /* Do nothing */ }
        public void LogError(string message, string tag = null) { /* Do nothing */ }

    }
}
