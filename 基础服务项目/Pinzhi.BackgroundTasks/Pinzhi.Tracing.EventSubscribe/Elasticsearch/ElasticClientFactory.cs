using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Linq;
using System.Threading;

namespace Pinzhi.Tracing.EventSubscribe.Elasticsearch
{
    public class ElasticClientFactory : IElasticClientFactory
    {
        private const string Default_ElasticsearchHosts = "http://localhost:9200";
        private readonly ElasticsearchOptions _elasticsearchOptions;
        private readonly Lazy<ElasticClient> _value;

        public ElasticClientFactory(IOptions<ElasticsearchOptions> options)
        {
            _elasticsearchOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _value = new Lazy<ElasticClient>(CreatElasticClient, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public ElasticClient Create()
        {
            return _value.Value;
        }

        private ElasticClient CreatElasticClient()
        {
            try
            {
                var elasticsearchHosts = string.IsNullOrEmpty(_elasticsearchOptions.ElasticsearchHosts) ? Default_ElasticsearchHosts : _elasticsearchOptions.ElasticsearchHosts;
                var urls = elasticsearchHosts.Split(';').Select(x => new Uri(x)).ToArray();
                // _logger.LogInformation($"Butterfly.Storage.Elasticsearch initialized ElasticClient with options: ElasticSearchHosts={elasticsearchHosts}.");
                var pool = new StaticConnectionPool(urls);
                var settings = new ConnectionSettings(pool);
                var client = new ElasticClient(settings);
                return client;
            }
            catch (Exception e)
            {
                // _logger.LogError(e, "Create ElasticClient failed.");
                throw;
            }

        }
    }
}
