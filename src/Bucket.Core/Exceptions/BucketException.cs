using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Exceptions
{
    public class BucketException : Exception
    {
        public BucketException(string code)
        {
            this.ErrorCode = code;
        }
        public BucketException(string code, string message) : base(message)
        {
            this.ErrorCode = code;
            this.ErrorMessage = message;
        }

        public BucketException(string code, string message, Exception exception) : base(message, exception)
        {
            this.ErrorCode = code;
            this.ErrorMessage = message + exception.Message;
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
