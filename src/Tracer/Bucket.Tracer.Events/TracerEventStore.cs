using Bucket.EventBus.Common.Events;
using System.Threading.Tasks;

namespace Bucket.Tracer.Events
{
    public class TracerEventStore : ITracerStore
    {
        private readonly IEventBus _eventBus;
        public TracerEventStore(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async Task Post(TraceLogs traceLogs)
        {
            await _eventBus.PublishAsync(new TracerEvent(traceLogs));
        }
    }
}
