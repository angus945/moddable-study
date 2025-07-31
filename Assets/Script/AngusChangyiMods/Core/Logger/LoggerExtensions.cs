namespace AngusChangyiMods.Logger
{
    public static class LoggerExtensions
    {
        public static void LogInfo(this ILogger logger, string message, string tag = null)
        {
            logger.Log(new LogInfo(LogLevel.Info, message, tag));
        }

        public static void LogError(this ILogger logger, string message, string tag = null)
        {
            logger.Log(new LogInfo(LogLevel.Error, message, tag));
        }

        public static void LogWarning(this ILogger logger, string message, string tag = null)
        {
            logger.Log(new LogInfo(LogLevel.Warning, message, tag));
        }
    }
}