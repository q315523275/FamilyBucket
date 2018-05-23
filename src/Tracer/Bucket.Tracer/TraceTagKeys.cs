using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracer
{
    public class TraceTagKeys
    {
        public const string Component = "component";
        public const string Environment = "environment";
        public const string Error = "error";
        public const string ContentType = "contenttype";
        public const string HttpMethod = "http.method";
        public const string HttpStatusCode = "http.status_code";
        public const string PeerAddress = "peer.address";
        public const string PeerPort = "peer.port";

        public const string RequestBody = "http.requestbody";
        public const string ResponseBody = "http.responsebody";
    }
}
