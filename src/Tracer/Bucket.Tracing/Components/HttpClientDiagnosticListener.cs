using Bucket.OpenTracing;
using Bucket.Tracing.Diagnostics;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Nito.AsyncEx;
namespace Bucket.Tracing.Components
{
    public class HttpClientDiagnosticListener : ITracingDiagnosticListener
    {
        private static Regex _tbbrRegex = new Regex(@"\s*|\t|\r|\n", RegexOptions.IgnoreCase);
        private readonly IServiceTracer _tracer;
        private readonly TracingOptions _options;

        public HttpClientDiagnosticListener(IServiceTracer tracer, IOptions<TracingOptions> options)
        {
            _tracer = tracer;
            _options = options.Value;
        }

        public string ListenerName { get; } = "HttpHandlerDiagnosticListener";

        [DiagnosticName("System.Net.Http.Request")]
        public void HttpRequest([Property(Name = "Request")] HttpRequestMessage request)
        {
            var patterns = _options.IgnoredRoutesRegexPatterns;
            if (patterns == null || patterns.Any(x => Regex.IsMatch(request.RequestUri.AbsolutePath, x)))
            {
                return;
            }
            var spanBuilder = new SpanBuilder($"httpclient {request.Method} {request.RequestUri.AbsolutePath}");
            var spanContext = _tracer.Tracer.GetEntrySpan()?.SpanContext;
            if (spanContext != null)
            {
                spanBuilder.AsChildOf(spanContext);
            }
            var span = _tracer.Start(spanBuilder);
            span.Tags.Client().Component("HttpClient")
                .HttpMethod(request.Method.Method)
                .HttpUrl(request.RequestUri.OriginalString)
                .HttpHost(request.RequestUri.Host)
                .HttpPath(request.RequestUri.PathAndQuery)
                .PeerAddress(request.RequestUri.OriginalString)
                .PeerHostName(request.RequestUri.Host)
                .PeerPort(request.RequestUri.Port);

            _tracer.Tracer.Inject(span.SpanContext, request.Headers, (c, k, v) => c.Add(k, v));
            span.Log(LogField.CreateNew().ClientSend());
            if(request.Method == HttpMethod.Post && _options.TraceHttpContent && request.Content != null)
            {
                var result = AsyncContext.Run(() => request.Content.ReadAsStringAsync());
                if (!string.IsNullOrWhiteSpace(result))
                    span.Tags.Add("http.request", _tbbrRegex.Replace(result, ""));
            }
                
            _tracer.Tracer.SetExitSpan(span);
        }

        [DiagnosticName("System.Net.Http.Response")]
        public void HttpResponse([Property(Name = "Response")] HttpResponseMessage response)
        {
            if (response == null)
                return;

            var span = _tracer.Tracer.GetExitSpan();
            if (span == null)
                return;

            span.Log(LogField.CreateNew().ClientReceive());
            span.Tags.HttpStatusCode((int)response.StatusCode);
            if (_options.TraceHttpContent && response.Content != null)
            {
                var result = AsyncContext.Run(() => response.Content.ReadAsStringAsync());
                if (result.Length > 2048)
                    result = result.Substring(0, 2048) + "...";
                if (!string.IsNullOrWhiteSpace(result))
                    span.Tags.Add("http.response", _tbbrRegex.Replace(result, ""));
            }
            span.Finish();
            _tracer.Tracer.SetExitSpan(null);
        }

        [DiagnosticName("System.Net.Http.Exception")]
        public void HttpException([Property(Name = "Request")] HttpRequestMessage request,
            [Property(Name = "Exception")] Exception ex)
        {
            var span = _tracer.Tracer.GetExitSpan();
            if (span == null)
            {
                return;
            }
            span.Exception(ex);
        }
    }
}
