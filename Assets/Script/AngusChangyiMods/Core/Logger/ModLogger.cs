namespace AngusChangyiMods.Core
{
    public class ModLogger
    {
        static ILogger logger;
        public ModLogger(ILogger logger)
        {
            ModLogger.logger = logger;
        }

        public static void Log(string message, string tag = null)
        {
            string formattedMessage = tag != null ? $"[{tag}] {message}" : message;
            logger?.Log(formattedMessage);
        }
        public static void LogError(string message, string tag = null)
        {
            string formattedMessage = tag != null ? $"[{tag}] {message}" : message;
            logger?.LogError(formattedMessage);
        }
        public static void LogWarning(string message, string tag = null)
        {
            string formattedMessage = tag != null ? $"[{tag}] {message}" : message;
            logger?.LogWarning(formattedMessage);
        }
    }
}
