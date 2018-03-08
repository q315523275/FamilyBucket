using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Bucket.ConfigCenter.Exceptions
{
    public class RemoteStatusCodeException : Exception
    {
        public RemoteStatusCodeException(HttpStatusCode statusCode, string message)
            : base(string.Format("[status code: {0:D}] {1}", statusCode, message))
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode
        {
            get;
            private set;
        }
    }
}
