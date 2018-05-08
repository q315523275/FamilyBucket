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
            Console.WriteLine(JsonConvert.SerializeObject(@event));
            var info = @event.TraceLog;
            if (info != null)
            {
                var index = new IndexName() { Name = $"tracer" };

                var indexRequest = new IndexRequest<TraceLogs>(info, index);

                var response = await _elasticClientManager._elasticClient.IndexAsync(indexRequest);
                if (!response.IsValid)
                {
                    // throw new ElasticsearchClientException("Add auditlog disaster!");
                }
            }
            return true;
        }

        public Task<bool> HandleAsync(IEvent @event, CancellationToken cancellationToken = default(CancellationToken))
            => CanHandle(@event) ? HandleAsync((TracerEvent)@event, cancellationToken) : Task.FromResult(false);
    }
}
