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
            logger?.Log(message, tag);
        }
        public static void LogError(string message, string tag = null)
        {
            logger?.LogError(message, tag);
        }
        public static void LogWarning(string message, string tag = null)
        {
            logger?.LogWarning(message, tag);
        }
    }
}
