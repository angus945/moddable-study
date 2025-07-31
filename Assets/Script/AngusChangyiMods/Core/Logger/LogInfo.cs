namespace AngusChangyiMods.Core
{
    public class LogInfo
    {   
        public LogLevel level;
        public string message;
        public string tag;
        public System.DateTime timestamp = System.DateTime.Now;
        
        public LogInfo(LogLevel level, string message, string tag = null)
        {
            this.level = level;
            this.message = message;
            this.tag = tag;
            timestamp = System.DateTime.Now;
        }

        public override string ToString()
        {
            return $"[{timestamp:yyyy-MM-dd HH:mm:ss}] [{level}] \n {message}" + (tag != null ? $" [Tag: {tag}]" : "");
        }
    }
}