using Bucket.Tracing.DataContract;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pinzhi.Tracing.EventSubscribe.Elasticsearch
{
    public class ElasticsearchSpanStorage : ISpanStorage
    {
        private readonly ElasticClient _elasticClient;
        private readonly IIndexManager _indexManager;

        public ElasticsearchSpanStorage(IElasticClientFactory elasticClientFactory, IIndexManager indexManager)
        {
            _elasticClient = elasticClientFactory?.Create() ?? throw new ArgumentNullException(nameof(elasticClientFactory));
            _indexManager = indexManager ?? throw new ArgumentNullException(nameof(indexManager));
        }

        public async Task StoreAsync(Span span, CancellationToken cancellationToken)
        {
            if (span == null)
                return;

            var index = _indexManager.CreateTracingIndex(DateTimeOffset.UtcNow);

            var response = await _elasticClient.IndexAsync(span, descriptor => descriptor.Index(index));

        }

        private async Task BulkStore(IEnumerable<Span> spans, CancellationToken cancellationToken)
        {
            var bulkRequest = new BulkRequest { Operations = new List<IBulkOperation>() };

            foreach (var span in spans)
            {
                var operation = new BulkIndexOperation<Span>(span) { Index = _indexManager.CreateTracingIndex(DateTimeOffset.UtcNow) };
                bulkRequest.Operations.Add(operation);
            }

            await _elasticClient.BulkAsync(bulkRequest, cancellationToken);
        }
    }
}
