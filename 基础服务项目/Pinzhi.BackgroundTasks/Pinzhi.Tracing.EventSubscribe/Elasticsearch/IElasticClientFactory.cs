using Nest;
namespace Pinzhi.Tracing.EventSubscribe.Elasticsearch
{
    public interface IElasticClientFactory
    {
        ElasticClient Create();
    }
}
