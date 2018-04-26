using Bucket.Buried.EventHandler.ElasticSearch;
using Bucket.EventBus.Common.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace Bucket.Buried.EventHandler
{
    public class BuriedEventHandler: IEventHandler<BuriedEvent>
    {
        private readonly ILogger logger;
        private readonly ESClientProvider _esClient;
        public BuriedEventHandler(
            ILogger<BuriedEventHandler> logger,
            ESClientProvider esClient)
        {
            this.logger = logger;
            _esClient = esClient;
        }

        public bool CanHandle(IEvent @event)
            => @event.GetType().Equals(typeof(BuriedEvent));

        public async Task<bool> HandleAsync(BuriedEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            var index = @event.buriedInformation;
            var res = await _esClient.Client.IndexAsync(index);
            return true;
        }

        public Task<bool> HandleAsync(IEvent @event, CancellationToken cancellationToken = default(CancellationToken))
            => CanHandle(@event) ? HandleAsync((BuriedEvent)@event, cancellationToken) : Task.FromResult(false);
    }
}
