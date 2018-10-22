using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.AspNetCore
{
    public class ErrorResult
    {
        public ErrorResult() { }
        public ErrorResult(string code, string msg)
        {
            Message = msg;
            ErrorCode = code;
        }

        public string Message { get; set; }
        public string ErrorCode { get; set; }
    }
}
