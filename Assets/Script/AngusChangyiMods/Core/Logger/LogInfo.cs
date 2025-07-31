using System;
using System.Diagnostics;

namespace AngusChangyiMods.Logger
{
    public class LogInfo
    {
        public LogLevel level;
        public string signature;
        public string message;
        public string tag;
        public DateTime timestamp;

        public LogInfo(LogLevel level, string message, string tag = null)
        {
            this.level = level;
            this.signature = GetCallerSignature();
            this.message = message;

            this.tag = tag;
            this.timestamp = DateTime.Now;
        }

        public override string ToString()
        {
             
            return $"[{timestamp:yyyy-MM-dd HH:mm:ss}] [{level}] [{signature}] " +
                   (tag != null ? $"[{tag}] " : "") +
                   $"\n{message}";
        }

        private string GetCallerSignature()
        {
            var trace = new StackTrace(skipFrames: 2, fNeedFileInfo: true);
            foreach (var frame in trace.GetFrames())
            {
                var method = frame.GetMethod();
                var type = method?.DeclaringType;

                if (type == null) continue;
                if (typeof(ILogger).IsAssignableFrom(type)) continue;
                if (type.Namespace != null && type.Namespace.StartsWith("AngusChangyiMods.Logger")) continue;

                return $"{type.Name}.{method.Name}";
            }

            return "UnknownCaller";
        }
    }
}