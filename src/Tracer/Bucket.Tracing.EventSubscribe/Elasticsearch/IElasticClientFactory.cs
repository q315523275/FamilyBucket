using Nest;
namespace Bucket.Tracing.EventSubscribe.Elasticsearch
{
    public interface IElasticClientFactory
    {
        ElasticClient Create();
    }
}
