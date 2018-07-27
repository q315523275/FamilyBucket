using Bucket.EventBus.Abstractions;
using Bucket.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Bucket.EventBus.InMemoryEventBusSubscriptionsManager;

namespace Bucket.EventBus
{
    public interface IEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;

        void AddSubscription<T, TH>()
           where T : IntegrationEvent
           where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>()
           where T : IntegrationEvent
           where TH : IIntegrationEventHandler<T>;

        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
        bool HasSubscriptionsForEvent(string eventName);
        void Clear();
        IEnumerable<Type> GetHandlersForEvent<T>() where T : IntegrationEvent;
        IEnumerable<Type> GetHandlersForEvent(string eventName);
        string GetEventKey<T>();
        Type GetEventTypeByName(string eventName);
    }
}
