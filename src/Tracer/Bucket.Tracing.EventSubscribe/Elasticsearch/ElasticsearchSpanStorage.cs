using Bucket.Tracing.DataContract;
using Nest;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Tracing.EventSubscribe.Elasticsearch
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

        public Task StoreAsync(IEnumerable<Span> spans, CancellationToken cancellationToken)
        {
            if (spans == null)
            {
                return Task.FromResult(false);
            }

            return BulkStore(spans, cancellationToken);
        }

        private Task BulkStore(IEnumerable<Span> spans, CancellationToken cancellationToken)
        {
            var bulkRequest = new BulkRequest { Operations = new List<IBulkOperation>() };

            foreach (var span in spans)
            {
                var operation = new BulkIndexOperation<Span>(span) { Index = _indexManager.CreateTracingIndex(DateTimeOffset.UtcNow) };
                bulkRequest.Operations.Add(operation);
            }

            //GeoDistanceQuery query = new GeoDistanceQuery
            //{
            //    Distance = new Distance(100, DistanceUnit.Meters),
            //    Location = new GeoLocation(30, 120),
            //    DistanceType = GeoDistanceType.SloppyArc,
            //    Field = new Field("")
            //};
            //GeoDistanceSort sort = new GeoDistanceSort()
            //{
            //    DistanceType = GeoDistanceType.SloppyArc,
            //    Field = new Field(""),
            //    GeoUnit = DistanceUnit.Meters,
            //    Order = SortOrder.Ascending,
            //    Points = new List<GeoLocation> { new GeoLocation(30, 120) }
            //};
            //SearchRequest search = new SearchRequest()
            //{
            //    Query = new QueryContainer(query),
            //    Sort = new List<ISort> { sort },
            //    Size = 200
            //};

            //_elasticClient.Search<object>(search).Documents.ToList();

            return _elasticClient.BulkAsync(bulkRequest, cancellationToken);
        }
    }
}
