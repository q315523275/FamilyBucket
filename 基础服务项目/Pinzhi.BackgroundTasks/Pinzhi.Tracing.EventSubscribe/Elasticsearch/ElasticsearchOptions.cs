using System;
using Microsoft.Extensions.Options;
namespace Pinzhi.Tracing.EventSubscribe.Elasticsearch
{
    public class ElasticsearchOptions : IOptions<ElasticsearchOptions>
    {
        public ElasticsearchOptions Value { get; }

        public string ElasticsearchHosts { get; set; }
    }
}
