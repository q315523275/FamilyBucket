using Bucket.Core;
using Bucket.Tracer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Threading.Tasks;

namespace Bucket.AspNetCore.Middleware.Tracer
{
    public class TracerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRequestScopedDataRepository _requestScopedDataRepository;
        private readonly ITracerHandler _tracer;
        public TracerMiddleware(RequestDelegate next, IRequestScopedDataRepository requestScopedDataRepository, ITracerHandler tracer)
        {
            _next = next;
            _requestScopedDataRepository = requestScopedDataRepository;
            _tracer = tracer;
        }
        public async Task Invoke(HttpContext context)
        {
            var trace = new TraceLogs()
            {
                ApiUri = context?.Request?.GetDisplayUrl(),
                StartTime = DateTime.Now,
                ContextType = context?.Request?.ContentType
            };
            _tracer.AddHeadersToTracer(context, trace);
            _requestScopedDataRepository.Add(TracerKeys.TraceStoreCacheKey, trace);

            await _next(context);

            trace = _requestScopedDataRepository.Get<TraceLogs>(TracerKeys.TraceStoreCacheKey);
            if (trace != null)
            {
                trace.ContextType = context?.Request?.ContentType;
                trace.EndTime = DateTime.Now;
                trace.TimeLength = Math.Round((trace.EndTime - trace.StartTime).TotalMilliseconds, 4);
                _requestScopedDataRepository.Update(TracerKeys.TraceStoreCacheKey, trace);
                await _tracer.PublishAsync(trace);
            }
        }
    }
}
