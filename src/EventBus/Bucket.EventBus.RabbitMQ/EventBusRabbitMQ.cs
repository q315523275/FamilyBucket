using Bucket.EventBus.Abstractions;
using Bucket.EventBus.Attributes;
using Bucket.EventBus.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bucket.EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private readonly string _exchangeName = "bucket_event_bus";

        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly IServiceProvider _autofac;
        private readonly EventBusRabbitMqOptions _options;
        private readonly int _retryCount;
        private readonly ushort _prefetchCount;

        private IDictionary<string, IModel> _consumerChannels;
        private readonly string _queueName;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger, IServiceProvider autofac,
            IEventBusSubscriptionsManager subsManager, IOptions<EventBusRabbitMqOptions> options)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager ?? throw new ArgumentNullException(nameof(subsManager));
            _options = options.Value;
            _exchangeName = _options.ExchangeName;
            _queueName = _options.QueueName;
            _consumerChannels = new Dictionary<string, IModel>();
            _autofac = autofac;
            _prefetchCount = _options.PrefetchCount;
            _retryCount = _options.RetryCount;
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }
        /// <summary>
        /// RabbitMQ取消订阅
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventName"></param>
        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {
                foreach (var key in _consumerChannels.Keys)
                {
                    if (_subsManager.IsEmpty)
                    {
                        _consumerChannels.TryGetValue(key, out var _consumerChannel);
                        if (_consumerChannel != null)
                        {
                            channel.QueueUnbind(queue: key,
                                                exchange: _exchangeName,
                                                routingKey: eventName);
                            _consumerChannel.Close();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 推送
        /// </summary>
        /// <param name="event"></param>
        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex.ToString());
                });

            using (var channel = _persistentConnection.CreateModel())
            {
                var eventName = @event.GetType().Name;

                channel.ExchangeDeclare(exchange: _exchangeName, type: "direct");

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent

                    channel.BasicPublish(exchange: _exchangeName, routingKey: eventName, mandatory: true, basicProperties: properties, body: body);
                });
            }
        }
        /// <summary>
        /// 订阅注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            var queueName = _queueName;
            var queueConsumerAttr = typeof(TH).GetCustomAttribute<QueueConsumerAttribute>();
            if(queueConsumerAttr != null)
                queueName = queueConsumerAttr.QueueName;
            DoInternalSubscription(eventName, queueName);
            _subsManager.AddSubscription<T, TH>();
        }
        private void DoInternalSubscription(string eventName, string queueName)
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!_persistentConnection.IsConnected)
                {
                    _persistentConnection.TryConnect();
                }

                if (!_consumerChannels.ContainsKey(queueName))
                {
                    _consumerChannels.Add(queueName, CreateConsumerChannel(queueName));
                }
                
                using (var channel = _persistentConnection.CreateModel())
                {
                    channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: eventName, arguments: new Dictionary<string, object> { { "x-queue-mode", "lazy" } });
                }
            }
        }
        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        public void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent
        {
            _subsManager.RemoveSubscription<T, TH>();
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            foreach (var key in _consumerChannels.Keys)
            {
                _consumerChannels.TryGetValue(key, out var _consumerChannel);
                if (_consumerChannel != null)
                {
                    _consumerChannel.Dispose();
                }
            }
            _subsManager.Clear();
        }
        /// <summary>
        /// 创建消费监听
        /// </summary>
        /// <returns></returns>
        private IModel CreateConsumerChannel(string queueName)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: _exchangeName, type: "direct");

            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object> { { "x-queue-mode", "lazy" } });

            return channel;
        }
        /// <summary>
        /// 存在未订阅已消费误差,故有之
        /// </summary>
        public void StartSubscribe()
        {
            foreach (var key in _consumerChannels.Keys)
            {
                _consumerChannels.TryGetValue(key, out var _consumerChannel);

                if (_consumerChannel == null || _consumerChannel.IsClosed)
                    _consumerChannel = CreateConsumerChannel(key);

                _consumerChannel.BasicQos(0, _prefetchCount, false);

                var consumer = new EventingBasicConsumer(_consumerChannel);

                _consumerChannel.BasicConsume(queue: key,
                         autoAck: false,
                         consumer: consumer);

                consumer.Received += async (model, ea) =>
                {
                    var eventName = ea.RoutingKey;
                    var message = Encoding.UTF8.GetString(ea.Body);

                    await ProcessEvent(eventName, message);

                    _consumerChannel.BasicAck(ea.DeliveryTag, multiple: false);
                };

                _consumerChannel.CallbackException += (sender, ea) =>
                {
                    _consumerChannel.Dispose();
                    _consumerChannel = CreateConsumerChannel(key);
                };
            }
        }
        /// <summary>
        /// 执行器
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task ProcessEvent(string eventName, string message)
        {
            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _autofac.CreateScope())
                {
                    var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        var eventType = _subsManager.GetEventTypeByName(eventName);
                        if (eventType != null)
                        {
                            var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                            var handler = scope.ServiceProvider.GetRequiredService(subscription);
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                            await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                        }
                    }
                }
            }
        }

    }
}
