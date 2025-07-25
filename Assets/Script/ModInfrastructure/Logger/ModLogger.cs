namespace ModArchitecture.Logger
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
            logger?.Log(message);
        }
        public static void LogError(string message, string tag = null)
        {
            logger?.LogError(message);
        }
        public static void LogWarning(string message, string tag = null)
        {
            logger?.LogWarning(message);
        }
    }
}
