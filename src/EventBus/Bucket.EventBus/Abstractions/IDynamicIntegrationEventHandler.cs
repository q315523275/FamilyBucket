using System.Threading.Tasks;

namespace Bucket.EventBus.Abstractions
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(string eventData);
    }
}
