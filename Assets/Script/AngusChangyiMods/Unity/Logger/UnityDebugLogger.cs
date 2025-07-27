using UnityEngine;
using AngusChangyiMods.Core;

namespace AngusChangyiMods.Unity
{
    public class UnityDebugLogger : AngusChangyiMods.Core.ILogger
    {
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
