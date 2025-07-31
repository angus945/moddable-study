using AngusChangyiMods.Logger;

namespace AngusChangyiMods.Unity
{
    public class UnityDebugLogger : ILogger
    {
        public void Log(LogInfo log)
        {
            switch (log.level)
            {
                case LogLevel.Info:
                    UnityEngine.Debug.Log(log);
                    break;
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(log);
                    break;
                case LogLevel.Error:
                    UnityEngine.Debug.LogError(log);
                    break;
            }
        }
    }
}