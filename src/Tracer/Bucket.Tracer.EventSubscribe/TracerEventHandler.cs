using Bucket.EventBus.Common.Events;
using Nest;
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Bucket.Tracer.Events;

namespace Bucket.Tracer.EventSubscribe
{
    public class TracerEventHandler : IEventHandler<TracerEvent>
    {
        private readonly ElasticClientManager _elasticClientManager;
        public TracerEventHandler(ElasticClientManager elasticClientManager)
        {
            _elasticClientManager = elasticClientManager;
        }

        public bool CanHandle(IEvent @event)
            => @event.GetType().Equals(typeof(TracerEvent));

        public async Task<bool> HandleAsync(TracerEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {

            var info = @event.TraceSpan;
            if (info != null)
            {
                var StartTime = DateTime.Now;

                var index = new IndexName() { Name = $"tracer" };

                var indexRequest = new IndexRequest<TraceSpan>(info, index);
                var response = await _elasticClientManager._elasticClient.IndexAsync(indexRequest);
                if (!response.IsValid)
                {
                    // throw new ElasticsearchClientException("Add auditlog disaster!");
                    Console.WriteLine("Add auditlog disaster!" + response.OriginalException);
                }
                var TimeLength = Math.Round((DateTime.Now - StartTime).TotalMilliseconds, 4);
                Console.WriteLine("es数据创建耗时"+ TimeLength + "毫秒");
            }
            return true;
        }

        public Task<bool> HandleAsync(IEvent @event, CancellationToken cancellationToken = default(CancellationToken))
            => CanHandle(@event) ? HandleAsync((TracerEvent)@event, cancellationToken) : Task.FromResult(false);
    }
}
