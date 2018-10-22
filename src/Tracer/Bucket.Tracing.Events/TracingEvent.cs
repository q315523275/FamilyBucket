using Bucket.EventBus.Events;
using Bucket.Tracing.DataContract;
using System;

namespace Bucket.Tracing.Events
{
    public class TracingEvent : IntegrationEvent
    {
        public TracingEvent(Span span)
        {
            this.TraceSpan = span;
        }
        public Span TraceSpan { get; set; }
    }
}
