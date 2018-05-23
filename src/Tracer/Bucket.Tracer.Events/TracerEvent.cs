using Bucket.EventBus.Common.Events;
using System;

namespace Bucket.Tracer.Events
{
    public class TracerEvent : IEvent
    {
        public TracerEvent(TraceSpan traceSpan)
        {
            this.Id = Guid.NewGuid();
            this.Timestamp = DateTime.UtcNow;
            this.TraceSpan = traceSpan;
        }

        public Guid Id { get; }
        public DateTime Timestamp { get; }
        public TraceSpan TraceSpan { get; set; }
    }
}
