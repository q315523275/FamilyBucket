using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracer.Events
{
    public class ElasticClientManager
    {
        private const string _alias = "bucket-tracer";
        private string _indexName = $"{_alias}-{DateTime.UtcNow.ToString("yyyy-MM")}";
        private static Field TimestampField = new Field("timestamp");

        public ElasticClient _elasticClient { get; }
        public ElasticClientManager()
        {
            var pool = new StaticConnectionPool(new List<Uri> { new Uri("http://127.0.0.1:9200") });
            var connectionSettings = new ConnectionSettings(
                pool,
                new HttpConnection(),
                new SerializerFactory((jsonSettings, nestSettings) => jsonSettings.Converters.Add(new StringEnumConverter())))
              .DisableDirectStreaming()
              .DefaultIndex("tracer");

            _elasticClient = new ElasticClient(connectionSettings);
        }
    }
}
