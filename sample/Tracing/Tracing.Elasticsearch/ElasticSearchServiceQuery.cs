using System.Collections.Generic;
using System.Threading.Tasks;
using Tracing.DataContract.Tracing;
using Tracing.Storage;
using Tracing.Storage.Query;
using Nest;

namespace Tracing.Elasticsearch
{
    internal class ElasticSearchServiceQuery : IServiceQuery
    {
        private readonly ElasticClient _elasticClient;
        private readonly IIndexManager _indexManager;

        public ElasticSearchServiceQuery(IElasticClientFactory elasticClientFactory, IIndexManager indexManager)
        {
            _indexManager = indexManager;
            _elasticClient = elasticClientFactory.Create();
        }

        public async Task<IEnumerable<Service>> GetServices(TimeRangeQuery query)
        {
            var index = _indexManager.CreateServiceIndex();
            var countResult = await _elasticClient.CountAsync<Service>(x => x.Index(index));
            var serviceResult = await _elasticClient.SearchAsync<Service>(s => s.Index(index).Size((int)countResult.Count));
            return serviceResult.Documents;
        }
    }
}