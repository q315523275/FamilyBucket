using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Bucket.ErrorCode.Util.Http
{
    public class HttpResponse<T>
    {
        public HttpResponse(HttpStatusCode statusCode, T body)
        {
            StatusCode = statusCode;
            Body = body;
        }

        public HttpResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
            Body = default(T);
        }

        public HttpStatusCode StatusCode
        {
            get;
            private set;

        }

        public T Body
        {
            get;
            private set;
        }
    }
}
