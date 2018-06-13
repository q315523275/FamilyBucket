using Bucket.OpenTracing;
using Bucket.Tracing.Diagnostics;
using Bucket.Tracing;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
namespace Bucket.Tracing.Components
{
    public class HttpClientDiagnosticListener : ITracingDiagnosticListener
    {
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
            var spanBuilder = new SpanBuilder($"httpclient {request.Method}");
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
            if(request.Method == HttpMethod.Post)
                span.Tags.Add("request", request.Content.ReadAsStringAsync().Result);

            _tracer.Tracer.SetExitSpan(span);
        }

        [DiagnosticName("System.Net.Http.Response")]
        public void HttpResponse([Property(Name = "Response")] HttpResponseMessage response)
        {
            var span = _tracer.Tracer.GetExitSpan();
            if (span == null)
            {
                return;
            }

            span.Log(LogField.CreateNew().ClientReceive());
            span.Tags.HttpStatusCode((int)response.StatusCode);

            var result = response.Content.ReadAsStringAsync().Result;
            if (result.Length > 2048)
                result = result.Substring(0, 2048) + "...";
            span.Tags.Add("response", result);

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
