using Bucket.EventBus.Common.Events;
using Nest;
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Bucket.Tracing.Events;
using Bucket.Tracing.EventSubscribe.Elasticsearch;
using System.Collections.Generic;
using Bucket.Tracing.DataContract;

namespace Bucket.Tracing.EventSubscribe
{
    public class TracingEventHandler : IEventHandler<TracingEvent>
    {
        private readonly ISpanStorage _spanStorage;
        public TracingEventHandler(ISpanStorage spanStorage)
        {
            _spanStorage = spanStorage;
        }

        public bool CanHandle(IEvent @event)
            => @event.GetType().Equals(typeof(TracingEvent));

        public async Task<bool> HandleAsync(TracingEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            /// 需要增加缓存区
            var span = @event.TraceSpan;
            if (span != null)
            {
                var StartTime = DateTime.Now;
                var spans = new List<Span>{ span };
                await _spanStorage.StoreAsync(spans);
                var TimeLength = Math.Round((DateTime.Now - StartTime).TotalMilliseconds, 4);
                Console.WriteLine("es数据创建耗时"+ TimeLength + "毫秒");
            }
            return true;
        }

        public Task<bool> HandleAsync(IEvent @event, CancellationToken cancellationToken = default(CancellationToken))
            => CanHandle(@event) ? HandleAsync((TracingEvent)@event, cancellationToken) : Task.FromResult(false);
    }
}
