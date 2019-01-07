using Bucket.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Bucket.EventBus.RabbitMQ.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IEventBusBuilder UseRabbitMQ(this IEventBusBuilder builder)
        {
            builder.Services.Configure<ConnectionFactory>(builder.Configuration.GetSection("EventBus:RabbitMQ"));
            builder.Services.AddSingleton<IRabbitMQPersistentConnection, DefaultRabbitMQPersistentConnection>();
            builder.Services.AddSingleton<IEventBus, EventBusRabbitMQ>();
            return builder;
        }
    }
}
