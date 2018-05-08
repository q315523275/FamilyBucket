using System.Threading;
using System.Threading.Tasks;

namespace Bucket.EventBus.Common.Events
{
    public interface IEventHandler
    {
        Task<bool> HandleAsync(IEvent @event, CancellationToken cancellationToken = default(CancellationToken));

        bool CanHandle(IEvent @event);
    }

    public interface IEventHandler<in T> : IEventHandler
        where T : IEvent
    {
        Task<bool> HandleAsync(T @event, CancellationToken cancellationToken = default(CancellationToken));
    }
}
