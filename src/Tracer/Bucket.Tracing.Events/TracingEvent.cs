using Bucket.EventBus.Common.Events;
using Bucket.Tracing.DataContract;
using System;

namespace Bucket.Tracing.Events
{
    public class TracingEvent : IEvent
    {
        public TracingEvent(Span span)
        {
            this.Id = Guid.NewGuid();
            this.Timestamp = DateTime.UtcNow;
            this.TraceSpan = span;
        }

        public Guid Id { get; }
        public DateTime Timestamp { get; }
        public Span TraceSpan { get; set; }
    }
}
