using Bucket.EventBus.Common.Events;
using System;

namespace Bucket.Tracer.Events
{
    public class TracerEvent : IEvent
    {
        public TracerEvent(TraceLogs traceLogs)
        {
            this.Id = Guid.NewGuid();
            this.Timestamp = DateTime.UtcNow;
            this.TraceLog = traceLogs;
        }

        public Guid Id { get; }
        public DateTime Timestamp { get; }
        public TraceLogs TraceLog { get; set; }
    }
}
