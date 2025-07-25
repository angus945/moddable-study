using UnityEngine;

namespace ModArchitecture.Logger
{
    public class UnityDebugLogger : ILogger
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }
        public void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }
        public void LogError(string message)
        {
            Debug.LogError(message);
        }
    }
}
