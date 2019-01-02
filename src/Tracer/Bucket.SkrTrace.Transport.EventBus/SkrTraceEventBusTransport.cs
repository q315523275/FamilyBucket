using Bucket.EventBus.Abstractions;
using Bucket.SkrTrace.Core.Abstractions;
using Bucket.SkrTrace.Core.Transport;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.SkrTrace.Transport.EventBus
{
    public class SkrTraceEventBusTransport : ISkrTraceCollect
    {
        private readonly IEventBus _eventBus;

        public SkrTraceEventBusTransport(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public Task CollectAsync(IEnumerable<TraceSegmentRequest> request, CancellationToken cancellationToken = default(CancellationToken))
        {
            _eventBus.Publish(new SkrTraceTransportEvent { TraceSegmentRequest = request });
            return Task.CompletedTask;
        }
    }
}
