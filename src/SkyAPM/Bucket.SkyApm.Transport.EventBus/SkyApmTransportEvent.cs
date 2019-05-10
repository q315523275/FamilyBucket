using System.Collections.Generic;
using Bucket.EventBus.Events;
namespace Bucket.SkyApm.Transport.EventBus
{
    public class SkyApmTransportEvent : IntegrationEvent
    {
        public IReadOnlyCollection<SegmentRequest> SegmentRequests { set; get; }
    }
}
