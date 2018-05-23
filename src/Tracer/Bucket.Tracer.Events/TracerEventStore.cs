using Bucket.EventBus.Common.Events;
using System.Threading.Tasks;

namespace Bucket.Tracer.Events
{
    public class TracerEventStore : ITraceSender
    {
        private readonly IEventBus _eventBus;
        public TracerEventStore(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async Task SendAsync(TraceSpan traceSpan)
        {
            await _eventBus.PublishAsync(new TracerEvent(traceSpan));
        }
    }
}
