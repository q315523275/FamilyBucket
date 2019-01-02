using Bucket.EventBus.Events;
using Bucket.SkrTrace.Core.Transport;
using System.Collections.Generic;

namespace Bucket.SkrTrace.Transport.EventBus
{
    public class SkrTraceTransportEvent : IntegrationEvent
    {
        public IEnumerable<TraceSegmentRequest> TraceSegmentRequest { set; get; }
    }
}
