using System;
using Microsoft.Extensions.Options;
namespace Bucket.Tracing.EventSubscribe.Elasticsearch
{
    public class ElasticsearchOptions : IOptions<ElasticsearchOptions>
    {
        public ElasticsearchOptions Value { get; }

        public string ElasticsearchHosts { get; set; }
    }
}
