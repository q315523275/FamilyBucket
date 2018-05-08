using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.EventBus.Common.Events
{
    public interface IEventPublisher : IDisposable
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default(CancellationToken))
            where TEvent : IEvent;
    }
}
