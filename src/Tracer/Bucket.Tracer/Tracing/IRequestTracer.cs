using System;
using Microsoft.AspNetCore.Http;
namespace Bucket.Tracer
{
    public interface IRequestTracer
    {

        TraceSpan OnBeginRequest(HttpContext httpContext);

        void OnEndRequest(HttpContext httpContext);

        void OnException(HttpContext httpContext, Exception exception, string @event);
    }
}
