using System;

namespace ModArchitecture
{
    /// <summary>
    /// 模組錯誤記錄
    /// </summary>
    public class ModError
    {
        public string ModId { get; set; }
        public ModErrorType ErrorType { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }

        public ModError(string modId, ModErrorType errorType, string message, Exception exception = null)
        {
            ModId = modId;
            ErrorType = errorType;
            Message = message;
            Exception = exception;
        }
    }
}
