using Bucket.OpenTracing;
using System;
using Newtonsoft.Json;
using Bucket.EventBus.Common.Events;

namespace Bucket.Tracing.Events
{
    public class EventSpanRecorder : ISpanRecorder
    {
        private readonly IEventBus _eventBus;
        public EventSpanRecorder(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Record(ISpan span)
        {
            _eventBus.PublishAsync(new TracingEvent(SpanContractUtils.CreateFromSpan(span)));
        }
    }
}
