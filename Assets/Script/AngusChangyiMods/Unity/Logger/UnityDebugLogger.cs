using UnityEngine;
using AngusChangyiMods.Core;

namespace AngusChangyiMods.Unity
{
    public class UnityDebugLogger : AngusChangyiMods.Core.ILogger
    {
        public void Log(LogInfo log)
        {
            switch (log.level)
            {
                case LogLevel.Info:
                    Debug.Log(log.message);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(log.message);
                    break;
                case LogLevel.Error:
                    Debug.LogError(log.message);
                    break;
                default:
                    Debug.Log(log.message);
                    break;
            }
        }
        public void Log(string message, string tag = null)
        {
            Debug.Log(message);
        }
        public void LogWarning(string message, string tag = null)
        {
            Debug.LogWarning(message);
        }
        public void LogError(string message, string tag = null)
        {
            Debug.LogError(message);
        }
    }
}
