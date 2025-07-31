using System;
using System.Diagnostics;

namespace AngusChangyiMods.Logger
{
    public class LogInfo
    {
        public readonly LogLevel Level;
        public readonly string Signature;
        public readonly string Message;
        public readonly string Tag;
        public readonly DateTime Timestamp;

        public LogInfo(LogLevel level, string message, string tag = null)
        {
            this.Level = level;
            this.Signature = GetCallerSignature();
            this.Message = message;

            this.Tag = tag;
            this.Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level}] [{Signature}] " +
                   (Tag != null ? $"[{Tag}] " : "") +
                   $"\n{Message}";
        }

        private string GetCallerSignature()
        {
            var trace = new StackTrace(skipFrames: 2, fNeedFileInfo: true);
            foreach (var frame in trace.GetFrames()!)
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

        public bool Contains(string contains)
        {
            return Message.Contains(contains, StringComparison.OrdinalIgnoreCase) || ContainsTag(contains);
        }
        public bool ContainsTag(string tag)
        {
            return Tag != null && Tag.Contains(tag, StringComparison.OrdinalIgnoreCase);
        }
    }
}