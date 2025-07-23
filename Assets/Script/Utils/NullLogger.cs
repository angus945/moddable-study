namespace ModdableArchitecture.Utils
{
    public class NullLogger : ILogger
    {
        public void Log(string message) { /* Do nothing */ }
        public void LogWarning(string message) { /* Do nothing */ }
        public void LogError(string message) { /* Do nothing */ }

    }
}
