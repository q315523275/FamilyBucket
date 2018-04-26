using Bucket.AspNetCore.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Bucket.EventBus.Common.Events;
using Bucket.EventBus.RabbitMQ;
using Microsoft.Extensions.Logging;

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

            services.AddSingleton<IEventBus>(sp => new RabbitMQEventBus(connectionFactory,
                sp.GetRequiredService<ILogger<RabbitMQEventBus>>(),
                sp.GetRequiredService<IEventHandlerExecutionContext>(),
                config.ExchangeName,
                queueName: config.QueueName,
                onlyPublish: config.OnlyPublish));
        }
    }
}
