using AngusChangyiMods.Core;
using System.Collections.Generic;

namespace ModInfrastructure.Test
{
    /// <summary>
    /// Mock logger for testing purposes that captures log messages
    /// </summary>
    public class MockLogger : ILogger
    {
        public List<string> LogMessages { get; } = new List<string>();
        public List<string> WarningMessages { get; } = new List<string>();
        public List<string> ErrorMessages { get; } = new List<string>();

        public void Log(string message, string tag = null)
        {
            string formattedMessage = tag != null ? $"[{tag}] {message}" : message;
            LogMessages.Add(formattedMessage);
        }

        public void LogWarning(string message, string tag = null)
        {
            string formattedMessage = tag != null ? $"[{tag}] {message}" : message;
            WarningMessages.Add(formattedMessage);
        }

        public void LogError(string message, string tag = null)
        {
            string formattedMessage = tag != null ? $"[{tag}] {message}" : message;
            ErrorMessages.Add(formattedMessage);
        }

        public void Clear()
        {
            LogMessages.Clear();
            WarningMessages.Clear();
            ErrorMessages.Clear();
        }
    }
}
