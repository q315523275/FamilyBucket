using Bucket.EventBus.Events;
using System.Collections.Generic;
namespace Bucket.SkyApm.Transport.EventBus
{
    public class SkyApmTransportEvent : IntegrationEvent
    {
        public IReadOnlyCollection<SegmentRequest> SegmentRequests { set; get; }
    }
}
