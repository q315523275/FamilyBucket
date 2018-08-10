using Bucket.OpenTracing;
using Bucket.Tracing.Diagnostics;
using Bucket.Tracing.Extensions;
using Bucket.Tracing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bucket.Tracing.Components
{
    public class HostingDiagnosticListener : ITracingDiagnosticListener
    {
        private readonly IServiceTracer _tracer;
        private readonly TracingOptions _options;
        public HostingDiagnosticListener(IServiceTracer tracer, IOptions<TracingOptions> options)
        {
            _tracer = tracer;
            _options = options.Value;
        }

        public string ListenerName { get; } = "Microsoft.AspNetCore";

        [DiagnosticName("Microsoft.AspNetCore.Hosting.BeginRequest")]
        public void BeginRequest([Property] HttpContext httpContext)
        {
            var patterns = _options.IgnoredRoutesRegexPatterns;
            if (patterns == null || patterns.Any(x => Regex.IsMatch(httpContext.Request.Path, x)))
            {
                return;
            }
            var spanBuilder = new SpanBuilder($"server {httpContext.Request.Method} {httpContext.Request.Path}");
            if (_tracer.Tracer.TryExtract(out var spanContext, httpContext.Request.Headers, (c, k) => c[k].GetValue(),
               c => c.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.GetValue())).GetEnumerator()))
            {
                spanBuilder.AsChildOf(spanContext);
            }
            var span = _tracer.Start(spanBuilder);
            span.Log(LogField.CreateNew().ServerReceive());
            span.Log(LogField.CreateNew().Event("AspNetCore BeginRequest"));
            span.Tags
             .Server().Component("AspNetCore")
             .HttpMethod(httpContext.Request.Method)
             .HttpUrl($"{httpContext.Request.Scheme}://{httpContext.Request.Host.ToUriComponent()}{httpContext.Request.Path}{httpContext.Request.QueryString}")
             .HttpHost(httpContext.Request.Host.ToUriComponent())
             .HttpPath(httpContext.Request.Path)
             .HttpStatusCode(httpContext.Response.StatusCode)
             .PeerAddress(httpContext.Connection.RemoteIpAddress.ToString())
             .PeerPort(httpContext.Connection.RemotePort)
             .UserIp(httpContext.GetUserIp());

            _tracer.Tracer.SetEntrySpan(span);
        }

        [DiagnosticName("Microsoft.AspNetCore.Hosting.EndRequest")]
        public void EndRequest([Property] HttpContext httpContext)
        {
            var span = _tracer.Tracer.GetEntrySpan();
            if (span == null)
            {
                return;
            }
            span.Tags.HttpStatusCode(httpContext.Response.StatusCode);

            span.Log(LogField.CreateNew().Event("AspNetCore EndRequest"));
            span.Log(LogField.CreateNew().ServerSend());
            span.Finish();
            _tracer.Tracer.SetEntrySpan(null);
        }

        [DiagnosticName("Microsoft.AspNetCore.Diagnostics.HandledException")]
        public void DiagnosticHandledException(HttpContext httpContext, Exception exception)
        {
            var span = _tracer.Tracer.GetEntrySpan();
            if (span == null)
            {
                return;
            }
            span.Log(LogField.CreateNew().Event("AspNetCore HandledException"));
            span.Exception(exception);
        }

        [DiagnosticName("Microsoft.AspNetCore.Diagnostics.UnhandledException")]
        public void DiagnosticUnhandledException([Property]HttpContext httpContext, [Property]Exception exception)
        {
            var span = _tracer.Tracer.GetEntrySpan();
            if (span == null)
            {
                return;
            }
            span.Log(LogField.CreateNew().Event("AspNetCore UnhandledException"));
            span.Exception(exception);
        }

        [DiagnosticName("Microsoft.AspNetCore.Hosting.UnhandledException")]
        public void HostingUnhandledException([Property]HttpContext httpContext, [Property]Exception exception)
        {
            var span = _tracer.Tracer.GetEntrySpan();
            if (span == null)
            {
                return;
            }
            span.Log(LogField.CreateNew().Event("AspNetCore UnhandledException"));
            span.Exception(exception);
        }
    }
}
