using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bucket.Tracer
{
    public interface ITracerHandler
    {
        void AddHeadersToTracer<T>(HttpContext httpContext, T traceLogs);
        Dictionary<string, string> DownTraceHeaders(HttpContext httpContext);
        Task PublishAsync<T>(T traceLogs);
    }
}
