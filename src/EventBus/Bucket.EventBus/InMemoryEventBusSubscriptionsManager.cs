using Bucket.EventBus.Abstractions;
using Bucket.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bucket.EventBus
{
    public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _eventTypes;

        public event EventHandler<string> OnEventRemoved;

        public InMemoryEventBusSubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<Type>>();
            _eventTypes = new List<Type>();
        }
        /// <summary>
        /// 是否空事件处理器
        /// </summary>
        public bool IsEmpty => !_handlers.Keys.Any();
        /// <summary>
        /// 清除事件处理器
        /// </summary>
        public void Clear() => _handlers.Clear();

        /// <summary>
        /// 新增订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            if (!HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<Type>());
            }
            _handlers[eventName].Add(typeof(TH));
            _eventTypes.Add(typeof(T));
        }

        /// <summary>
        /// 移除订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        public void RemoveSubscription<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent
        {
            var handlerToRemove = FindSubscriptionToRemove<T, TH>();
            var eventName = GetEventKey<T>();
            if (handlerToRemove != null)
            {
                _handlers[eventName].Remove(handlerToRemove);
                if (!_handlers[eventName].Any())
                {
                    _handlers.Remove(eventName);
                    var eventType = _eventTypes.FirstOrDefault(e => e.Name == eventName);
                    if (eventType != null)
                    {
                        _eventTypes.Remove(eventType);
                    }
                    RaiseOnEventRemoved(eventName);
                }

            }
        }
        /// <summary>
        /// 根据事件获得事件处理器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<Type> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return GetHandlersForEvent(key);
        }
        /// <summary>
        /// 根据事件名获得事件处理器
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public IEnumerable<Type> GetHandlersForEvent(string eventName) => _handlers[eventName];

        /// <summary>
        /// 取消事件引发
        /// </summary>
        /// <param name="eventName"></param>
        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            if (handler != null)
            {
                OnEventRemoved(this, eventName);
            }
        }

        /// <summary>
        /// 查询指定事件及事件处理器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <returns></returns>
        private Type FindSubscriptionToRemove<T, TH>()
             where T : IntegrationEvent
             where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            return DoFindSubscriptionToRemove(eventName, typeof(TH));
        }
        /// <summary>
        /// 查询事件处理器
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        private Type DoFindSubscriptionToRemove(string eventName, Type handlerType)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                return null;
            }

            return _handlers[eventName].FirstOrDefault(s => s == handlerType);

        }
        /// <summary>
        /// 是否含有事件订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return HasSubscriptionsForEvent(key);
        }
        /// <summary>
        /// 是否含有事件订阅
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);
        /// <summary>
        /// 获取事件名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string GetEventKey<T>()
        {
            return typeof(T).Name;
        }
        public Type GetEventTypeByName(string eventName) => _eventTypes.FirstOrDefault(t => t.Name == eventName);
    }
}
