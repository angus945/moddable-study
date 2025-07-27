using UnityEngine;
using AngusChangyiMods.Core;

namespace AngusChangyiMods.Unity
{
    public class UnityDebugLogger : AngusChangyiMods.Core.ILogger
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
