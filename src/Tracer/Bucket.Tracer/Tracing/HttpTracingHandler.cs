using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Tracer
{
    public class HttpTracingHandler : DelegatingHandler
    {
        private readonly IServiceTracer _tracer;

        public HttpTracingHandler(IServiceTracer tracer, HttpMessageHandler httpMessageHandler = null)
        {
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
            InnerHandler = httpMessageHandler ?? new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var span = _tracer.Start();

            _tracer.AsChildren(span);

            span.SetOperationName(request.RequestUri.OriginalString);
            span.Tags.Component("HttpClient")
                .HttpMethod(request.Method.Method);

            if (request.Method.Method.ToLower() == "post")
                span.Tags.RequstBody(request.Content.ReadAsStringAsync().Result);

            request.Headers.Add(TracerKeys.TraceId, span.TraceId);
            request.Headers.Add(TracerKeys.TraceSpanId, span.SpanId);
            request.Headers.Add(TracerKeys.TraceLaunchId, span.LaunchId);

            var responseMessage = await base.SendAsync(request, cancellationToken);

            var statusCode = responseMessage.StatusCode;
            span.Tags.HttpStatusCode((int)statusCode);
            if (statusCode == System.Net.HttpStatusCode.OK)
            {
                span.Tags
                    .ResponseBody(responseMessage.Content.ReadAsStringAsync().Result)
                    .ContentType(responseMessage.Content.Headers.ContentType.MediaType);
            }
            span.Finish();

            await _tracer.TraceSender.SendAsync(span);

            return responseMessage;
        }
    }
}
