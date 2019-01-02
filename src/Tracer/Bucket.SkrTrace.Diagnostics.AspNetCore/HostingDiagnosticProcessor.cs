using Bucket.SkrTrace.Core;
using Bucket.SkrTrace.Core.Abstractions;
using Bucket.SkrTrace.Core.Diagnostics;
using Bucket.SkrTrace.Core.Extensions;
using Bucket.SkrTrace.Core.Implementation;
using Bucket.SkrTrace.Core.OpenTracing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bucket.SkrTrace.Diagnostics.AspNetCore
{
    public class HostingDiagnosticProcessor: ITracingDiagnosticProcessor
    {
        public string ListenerName { get; } = "Microsoft.AspNetCore";
        private readonly IContextCarrierFactory _contextCarrierFactory;
        private readonly SkrTraceOptions _skrTraceOptions;

        public HostingDiagnosticProcessor(IContextCarrierFactory contextCarrierFactory, IOptions<SkrTraceOptions> skrTraceOptions)
        {
            _contextCarrierFactory = contextCarrierFactory;
            _skrTraceOptions = skrTraceOptions.Value;
        }

        [DiagnosticName("Microsoft.AspNetCore.Hosting.BeginRequest")]
        public void BeginRequest([Property] HttpContext httpContext)
        {
            if (IgnoredRoute(httpContext.Request.Path))
                return;

            var carrier = _contextCarrierFactory.Create();
            foreach (var item in carrier.Items)
                item.HeadValue = httpContext.Request.Headers[item.HeadKey];

            var httpRequestSpan = ContextManager.CreateEntrySpan($"{_skrTraceOptions.ApplicationCode} {httpContext.Request.Path}", carrier);
            httpRequestSpan.AsHttp();
            httpRequestSpan.SetComponent(SpanComponent.AspNetCore);
            httpRequestSpan.Tags.Server()
                .HttpMethod(httpContext.Request.Method)
                .HttpUrl(httpContext.Request.GetDisplayUrl());

            httpRequestSpan.Log(LogField.CreateNew().Event("AspNetCore Hosting BeginRequest")
                                        .Message($"Request starting {httpContext.Request.Protocol} {httpContext.Request.Method} {httpContext.Request.GetDisplayUrl()}"));
        }

        [DiagnosticName("Microsoft.AspNetCore.Hosting.EndRequest")]
        public void EndRequest([Property] HttpContext httpContext)
        {
            var httpRequestSpan = ContextManager.ActiveSpan;
            if (httpRequestSpan == null)
            {
                return;
            }

            var statusCode = httpContext.Response.StatusCode;
            if (statusCode >= 400)
            {
                httpRequestSpan.ErrorOccurred();
            }

            httpRequestSpan.Tags.HttpStatusCode(statusCode);

            httpRequestSpan.Log(LogField.CreateNew().Event("AspNetCore Hosting EndRequest")
                                                    .Message($"Request finished {httpContext.Response.StatusCode} {httpContext.Response.ContentType}"));

            ContextManager.StopSpan(httpRequestSpan);
        }

        [DiagnosticName("Microsoft.AspNetCore.Diagnostics.UnhandledException")]
        public void DiagnosticUnhandledException([Property]HttpContext httpContext, [Property]Exception exception)
        {
            var httpRequestSpan = ContextManager.ActiveSpan;
            if (httpRequestSpan == null)
            {
                return;
            }
            httpRequestSpan.Exception(exception);
        }

        [DiagnosticName("Microsoft.AspNetCore.Hosting.UnhandledException")]
        public void HostingUnhandledException([Property]HttpContext httpContext, [Property]Exception exception)
        {
            var httpRequestSpan = ContextManager.ActiveSpan;
            if (httpRequestSpan == null)
            {
                return;
            }
            httpRequestSpan.Exception(exception);
        }

        private bool IgnoredRoute(string routePath)
        {
            var patterns = _skrTraceOptions.IgnoredRoutesRegexPatterns;
            if (patterns != null && patterns.Length > 0 && patterns.Any(x => Regex.IsMatch(routePath, x, RegexOptions.IgnoreCase)))
                return true;
            return false;
        }
    }
}
