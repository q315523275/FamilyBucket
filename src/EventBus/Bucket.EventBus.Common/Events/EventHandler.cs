using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.EventBus.Common.Events
{
    public abstract class EventHandler<T> : IEventHandler<T>
        where T : IEvent
    {
        public bool CanHandle(IEvent @event)
            => typeof(T).Equals(@event.GetType());

        public abstract Task<bool> HandleAsync(T @event, CancellationToken cancellationToken = default(CancellationToken));

        public Task<bool> HandleAsync(IEvent @event, CancellationToken cancellationToken = default(CancellationToken))
            => CanHandle(@event) ? HandleAsync((T)@event, cancellationToken) : Task.FromResult(false);
    }
}
