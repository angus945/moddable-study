namespace AngusChangyiMods.Core
{
    public interface ILogger
    {
        void Log(string message, string tag = null);
        void LogError(string message, string tag = null);
        void LogWarning(string message, string tag = null);
    }
}
