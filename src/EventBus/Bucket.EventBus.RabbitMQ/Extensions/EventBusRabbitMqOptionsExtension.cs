using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Bucket.EventBus.Abstractions;
using Bucket.DependencyInjection;
using RabbitMQ.Client;
namespace Bucket.EventBus.RabbitMQ
{
    public class EventBusRabbitMqOptionsExtension : IOptionsExtension
    {
        private readonly EventBusRabbitMqOptions _options;
        public EventBusRabbitMqOptionsExtension(EventBusRabbitMqOptions options)
        {
            _options = options;
        }

        public void AddServices(IServiceCollection services)
        {
            var connectionFactory = new ConnectionFactory {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                AutomaticRecoveryEnabled = true
            };
            services.AddSingleton<IRabbitMQPersistentConnection>(sp => new DefaultRabbitMQPersistentConnection(
                connectionFactory, 
                sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>()
            ));
            services.AddSingleton<IEventBus>(sp => new EventBusRabbitMQ(
                sp.GetRequiredService<IRabbitMQPersistentConnection>(), 
                sp.GetRequiredService<ILogger<EventBusRabbitMQ>>(),
                sp,
                sp.GetRequiredService<IEventBusSubscriptionsManager>(), 
                _options.QueueName,
                prefetchCount: _options.PrefetchCount,
                retryCount: 5
            ));
        }
    }
}
