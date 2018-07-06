using Bucket.OpenTracing;
using System;
using Newtonsoft.Json;
using Bucket.EventBus.Abstractions;

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
            _eventBus.Publish(new TracingEvent(SpanContractUtils.CreateFromSpan(span)));
        }
    }
}
