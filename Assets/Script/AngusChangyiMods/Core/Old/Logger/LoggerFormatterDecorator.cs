namespace AngusChangyiMods.Core
{
    public class LoggerFormatterDecorator : ILogger
    {
        private readonly ILogger inner;
        public LoggerFormatterDecorator(ILogger inner)
        {
            this.inner = inner;
        }
        public void Log(string message, string tag = null)
        {
            string formattedMessage = tag != null ? $"[{tag}] {message}" : message;
            inner.Log(formattedMessage);
        }
        public void LogError(string message, string tag = null)
        {
            string formattedMessage = tag != null ? $"[{tag}] {message}" : message;
            inner.LogError(formattedMessage);
        }
        public void LogWarning(string message, string tag = null)
        {
            string formattedMessage = tag != null ? $"[{tag}] {message}" : message;
            inner.LogWarning(formattedMessage);
        }
    }
}
