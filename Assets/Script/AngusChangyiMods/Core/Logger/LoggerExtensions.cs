namespace AngusChangyiMods.Logger
{
    public static class LoggerExtensions
    {
        public static void LogInfo(this ILogger logger, string message)
        {
            logger.Log(new LogInfo(LogLevel.Info, message));
        }

        public static void LogError(this ILogger logger, string message)
        {
            logger.Log(new LogInfo(LogLevel.Error, message));
        }

        public static void LogWarning(this ILogger logger, string message)
        {
            logger.Log(new LogInfo(LogLevel.Warning, message));
        }
    }
}