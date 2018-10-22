using Nest;

namespace Tracing.Elasticsearch
{
    public interface IElasticClientFactory
    {
        ElasticClient Create();
    }
}