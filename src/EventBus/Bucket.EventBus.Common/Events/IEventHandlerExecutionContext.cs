using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.EventBus.Common.Events
{
    public interface IEventHandlerExecutionContext
    {
        void RegisterHandler<TEvent, THandler>()
            where TEvent : IEvent
            where THandler : IEventHandler<TEvent>;

        void RegisterHandler(Type eventType, Type handlerType);

        bool HandlerRegistered<TEvent, THandler>()
            where TEvent : IEvent
            where THandler : IEventHandler<TEvent>;

        bool HandlerRegistered(Type eventType, Type handlerType);

        Task HandleEventAsync(IEvent @event, CancellationToken cancellationToken = default(CancellationToken));
    }
}
