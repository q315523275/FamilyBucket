using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracer.EventSubscribe
{
    public class ElasticClientManager
    {
        public ElasticClient _elasticClient { get; }
        public ElasticClientManager(ElasticClientOptions setting)
        {
            var pool = new StaticConnectionPool(new List<Uri> { new Uri(setting.Server) });
            var connectionSettings = new ConnectionSettings(
                pool,
                new HttpConnection(),
                new SerializerFactory((jsonSettings, nestSettings) => jsonSettings.Converters.Add(new StringEnumConverter())))
              .DisableDirectStreaming()
              .DefaultIndex(setting.DefaultIndex);

            _elasticClient = new ElasticClient(connectionSettings);
        }
    }
}
