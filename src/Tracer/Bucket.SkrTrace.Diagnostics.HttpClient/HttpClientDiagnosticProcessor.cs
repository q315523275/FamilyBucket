using Bucket.SkrTrace.Core;
using Bucket.SkrTrace.Core.Abstractions;
using Bucket.SkrTrace.Core.Diagnostics;
using Bucket.SkrTrace.Core.Extensions;
using Bucket.SkrTrace.Core.Implementation;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Bucket.SkrTrace.Diagnostics.HttpClient
{
    public class HttpClientDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName { get; } = "HttpHandlerDiagnosticListener";
        private readonly IContextCarrierFactory _contextCarrierFactory;
        private readonly SkrTraceOptions _skrTraceOptions;

        public HttpClientDiagnosticProcessor(IContextCarrierFactory contextCarrierFactory, IOptions<SkrTraceOptions> skrTraceOptions)
        {
            _contextCarrierFactory = contextCarrierFactory;
            _skrTraceOptions = skrTraceOptions.Value;
        }

        [DiagnosticName("System.Net.Http.Request")]
        public void HttpRequest([Property(Name = "Request")] HttpRequestMessage request)
        {
            var contextCarrier = _contextCarrierFactory.Create();
            var peer = $"{request.RequestUri.Host}:{request.RequestUri.Port}";
            var span = ContextManager.CreateExitSpan(request.RequestUri.AbsolutePath, contextCarrier, peer);
            span.AsHttp();
            span.SetComponent(SpanComponent.HttpClient);
            span.Tags.Client()
               .HttpMethod(request.Method.Method)
               .HttpUrl(request.RequestUri.OriginalString);
            if (_skrTraceOptions.TraceHttpBody && request.Method == HttpMethod.Post && request.Content != null)
                span.Tags.HttpRequest(request.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult());
            foreach (var item in contextCarrier.Items)
                request.Headers.Add(item.HeadKey, item.HeadValue);
        }

        [DiagnosticName("System.Net.Http.Response")]
        public void HttpResponse([Property(Name = "Response")] HttpResponseMessage response)
        {
            var span = ContextManager.ActiveSpan;
            if (span != null && response != null)
            {
                span.Tags.HttpStatusCode((int)response.StatusCode);
                if (_skrTraceOptions.TraceHttpBody && response.StatusCode == HttpStatusCode.OK && response.Content != null)
                    span.Tags.HttpResponse(response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult());
            }
            ContextManager.StopSpan(span);
        }

        [DiagnosticName("System.Net.Http.Exception")]
        public void HttpException([Property(Name = "Request")] HttpRequestMessage request, [Property(Name = "Exception")] Exception ex)
        {
            var span = ContextManager.ActiveSpan;
            if (span != null && span.IsExit)
            {
                span.Exception(ex);
            }
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
