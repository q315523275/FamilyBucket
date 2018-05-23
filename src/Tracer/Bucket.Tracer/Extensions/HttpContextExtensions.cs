using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
namespace Bucket.Tracer
{
    public static class HttpContextExtensions
    {
        private const string spanKey = "_request_trace_span_";

        public static void SetSpan(this HttpContext httpContext, TraceSpan span)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            httpContext.Items[spanKey] = span;
        }

        public static TraceSpan GetSpan(this HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            return httpContext.Items[spanKey] as TraceSpan;
        }
    }
}
