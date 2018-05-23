using Bucket.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bucket.Tracer
{
    public class ServiceTracer : IServiceTracer
    {
        private readonly ITraceSender _traceSender;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TraceOptions _options;
        public ServiceTracer(IHttpContextAccessor httpContextAccessor, 
            IOptions<TraceOptions> options,
            ITraceSender traceSender)
        {
            _traceSender = traceSender;
            _httpContextAccessor = httpContextAccessor;
            _options = options.Value;
        }

        public ITraceSender TraceSender => _traceSender;

        public TraceSpan Start()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var span = new TraceSpan
            {
                SpanId = Guid.NewGuid().ToString("N"),
                StartTime = DateTime.Now,
                SystemName = _options.SystemName,
                Tags = new TraceTags { { TraceTagKeys.Environment, _options.Environment } }
            };
            if (httpContext.Request.Headers.ContainsKey(TracerKeys.TraceId))
                span.TraceId = httpContext.Request.Headers[TracerKeys.TraceId].FirstOrDefault();
            else
                span.TraceId = Guid.NewGuid().ToString("N");

            if (httpContext.Request.Headers.ContainsKey(TracerKeys.TraceSpanId))
                span.ParentId = httpContext.Request.Headers[TracerKeys.TraceSpanId].FirstOrDefault();
            else
                span.ParentId = "0";

            if (httpContext.Request.Headers.ContainsKey(TracerKeys.TraceLaunchId))
                span.LaunchId = httpContext.Request.Headers[TracerKeys.TraceLaunchId].FirstOrDefault();

            return span;
        }

        public void AsChildren(TraceSpan span)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var curSpan = httpContext.GetSpan();
            if (curSpan == null)
                return;
            span.ParentId = curSpan.SpanId;
            span.LaunchId = curSpan.LaunchId;
        }

    }
}
