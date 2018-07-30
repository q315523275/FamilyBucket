using Bucket.Tracing.DataContract;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace Bucket.Tracing.EventSubscribe.Elasticsearch
{
    public class IndexManager : IIndexManager
    {
        private const string TracingIndexSuffix = "pinzhi-tracing";
        private const string ServiceIndexSuffix = "pinzhi-service";

        private readonly IMemoryCache _memoryCache;
        private readonly ElasticClient _elasticClient;
        private ILogger _logger;

        public IndexManager(IMemoryCache memoryCache, IElasticClientFactory elasticClientFactory, ILogger<IndexManager> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _elasticClient = elasticClientFactory.Create();

            Console.WriteLine("ElasticsearchIndex初始化");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IndexName CreateServiceIndex()
        {
            var index = ServiceIndexSuffix;
            return GetOrCreateIndex(index, IndexExists, CreateServiceIndexExecute);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IndexName CreateTracingIndex(DateTimeOffset? dateTimeOffset = null)
        {
            if (dateTimeOffset == null)
            {
                return $"{TracingIndexSuffix}-*";
            }

            var index = $"{TracingIndexSuffix}-{dateTimeOffset.Value:yyyyMM}";

            return GetOrCreateIndex(index, IndexExists, CreateTracingIndexExecute);
        }

        private string GetOrCreateIndex(string index, Func<string, bool> predicate, Action<string> factory)
        {
            if (_memoryCache.TryGetValue(index, out _))
            {
                return index;
            }

            if (!predicate(index))
            {
                factory(index);
            }

            _memoryCache.Set<bool>(index, true, TimeSpan.FromHours(1));
            return index;
        }

        private bool IndexExists(string index)
        {
            return _elasticClient.IndexExists(Indices.Index(index)).Exists;
        }

        private void CreateTracingIndexExecute(string index)
        {
            _logger.LogInformation($"Not exists index {index}.");

            var tracingIndex = new CreateIndexDescriptor(index);

            tracingIndex.Mappings(x =>
                x.Map<Span>(m => m
                    .AutoMap()
                    .Properties(p => p.Keyword(t => t.Name(n => n.TraceId)))
                    .Properties(p => p.Keyword(t => t.Name(n => n.SpanId)))
                    .Properties(p => p.Nested<Tag>(n => n.Name(name => name.Tags).AutoMap()))));

            var response = _elasticClient.CreateIndex(tracingIndex);

            if (response.IsValid)
            {
                _logger.LogInformation($"Create index {index} success.");
            }
            else
            {
                var exception = new InvalidOperationException($"Create index {index} error : {response.ServerError}");
                _logger.LogError(exception, exception.Message);
                throw exception;
            }
        }

        private void CreateServiceIndexExecute(string index)
        {
            _logger.LogInformation($"Not exists index {index}.");

            var serviceIndex = new CreateIndexDescriptor(index);

            serviceIndex.Mappings(x =>
               x.Map<Service>(m => m
                    .Properties(p => p.Keyword(k => k.Name(n => n.Name)))));

            var response = _elasticClient.CreateIndex(serviceIndex);

            if (response.IsValid)
            {
                _logger.LogInformation($"Create index {index} success.");
            }
            else
            {
                var exception = new InvalidOperationException($"Create index {index} error : {response.ServerError}");
                _logger.LogError(exception, exception.Message);
                throw exception;
            }
        }
    }
}
