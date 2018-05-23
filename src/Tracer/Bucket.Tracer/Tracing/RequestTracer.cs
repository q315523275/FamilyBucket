using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;

namespace Bucket.Tracer
{
    public class RequestTracer : IRequestTracer
    {
        private readonly IServiceTracer _tracer;
        private readonly TraceOptions _options;

        public RequestTracer(IServiceTracer tracer, IOptions<TraceOptions> options)
        {
            _tracer = tracer;
            _options = options.Value;
        }
        public TraceSpan OnBeginRequest(HttpContext httpContext)
        {
            var patterns = _options.IgnoredRoutesRegexPatterns;
            if (patterns == null || patterns.Any(x => Regex.IsMatch(httpContext.Request.Path, x)))
            {
                return null;
            }
            var span = _tracer.Start();
            span.SetOperationName(httpContext.Request.GetDisplayUrl());
            span.Tags
                .Component("AspNetCore")
                .HttpMethod(httpContext.Request.Method)
                .PeerAddress(httpContext.Connection.RemoteIpAddress.ToString())
                .PeerPort(httpContext.Connection.RemotePort);

            httpContext.SetSpan(span);
            return span;
        }

        public void OnEndRequest(HttpContext httpContext)
        {
            var span = httpContext.GetSpan();
            if (span == null)
            {
                return;
            }
            span.Tags.HttpStatusCode(httpContext.Response.StatusCode);
            span.Finish();
            _tracer.TraceSender.SendAsync(span);
        }

        public void OnException(HttpContext httpContext, Exception exception, string @event)
        {
            var span = httpContext.GetSpan();
            if (span == null)
            {
                return;
            }
            span.Exception(exception);
        }
    }
}
