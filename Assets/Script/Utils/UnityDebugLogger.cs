using UnityEngine;

namespace ModdableArchitecture.Utils
{
    public class UnityDebugLogger : ILogger
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }
    }
}
