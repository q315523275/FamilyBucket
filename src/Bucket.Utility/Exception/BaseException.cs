using Bucket.Extend;
using System;

namespace Bucket.Base
{
    public class BaseException : Exception
    {
        public BaseException(bool is_alarm = false)
        {
            this.IsAlarm = is_alarm;
        }
        public BaseException(string code, bool is_alarm = false)
        {
            this.IsAlarm = is_alarm;
            this.ErrorCode = code;
        }
        public BaseException(string code, string message, bool is_alarm = false) : base(message)
        {
            this.IsAlarm = is_alarm;
            this.ErrorCode = code;
            this.ErrorMessage = message;
        }

        public BaseException(string code, string message, Exception exception, bool is_alarm = false) : base(message, exception)
        {
            this.IsAlarm = is_alarm;
            this.ErrorCode = code;
            this.ErrorMessage = message + exception.Message.NullToEmpty();
        }
        /// <summary>
        /// 错误码
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// 错误描述(对外)
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 是否发出警告
        /// </summary>
        public bool IsAlarm { get; set; }
    }
}
