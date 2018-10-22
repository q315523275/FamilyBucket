using System;

namespace Bucket.Exceptions
{
    public class BucketException : Exception
    {
        public BucketException(string code)
        {
            ErrorCode = code;
        }
        public BucketException(string code, string message) : base(message)
        {
            ErrorCode = code;
            ErrorMessage = message;
        }

        public BucketException(string code, string message, Exception exception) : base(message, exception)
        {
            ErrorCode = code;
            ErrorMessage = message + exception.Message;
        }
        /// <summary>
        /// 错误码
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// 错误描述
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
