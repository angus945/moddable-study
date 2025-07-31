namespace AngusChangyiMods.Core
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }
    public interface ILogger
    {
        void Log(LogInfo logInfo);
        void Log(string message, string tag = null);
        void LogError(string message, string tag = null);
        void LogWarning(string message, string tag = null);
    }
}
