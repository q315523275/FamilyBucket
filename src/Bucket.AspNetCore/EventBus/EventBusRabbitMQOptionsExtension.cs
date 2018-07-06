using Bucket.AspNetCore.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Bucket.EventBus.RabbitMQ;
using Microsoft.Extensions.Logging;
using Bucket.EventBus;
using Bucket.EventBus.Abstractions;

namespace Bucket.AspNetCore.EventBus
{
    public class EventBusRabbitMQOptionsExtension : IOptionsExtension
    {
        private readonly Action<EventBusRabbitMQOptions> _configure;
        public EventBusRabbitMQOptionsExtension(Action<EventBusRabbitMQOptions> configure)
        {
            _configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            var config = new EventBusRabbitMQOptions();
            _configure?.Invoke(config);

            var connectionFactory = new ConnectionFactory {
                HostName = config.HostName,
                Port = config.Port,
                UserName = config.UserName,
                Password = config.Password,
                VirtualHost = config.VirtualHost,
                AutomaticRecoveryEnabled = true
            };
            services.AddSingleton<IRabbitMQPersistentConnection>(sp => new DefaultRabbitMQPersistentConnection(connectionFactory, sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>()));
            services.AddSingleton<IEventBus>(sp => new EventBusRabbitMQ(sp.GetRequiredService<IRabbitMQPersistentConnection>(), sp.GetRequiredService<ILogger<EventBusRabbitMQ>>(), sp.GetRequiredService<IEventBusSubscriptionsManager>(), config.QueueName));
        }
    }
}
