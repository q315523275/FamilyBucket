using Bucket.EventBus.Abstractions;
using Bucket.EventBus.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Bucket.EventBus.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加事件消息总线
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEventBusBuilder AddEventBus(this IServiceCollection services, Action<IEventBusBuilder> action)
        {
            var service = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)service.ImplementationInstance;
            var builder = new EventBusBuilder(services, configuration);
            action(builder);
            return builder;
        }
        /// <summary>
        /// 添加事件消息总线
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEventBusBuilder AddEventBus(this IServiceCollection services, IConfiguration configuration, Action<IEventBusBuilder> action)
        {
            var builder = new EventBusBuilder(services, configuration);
            action(builder);
            return builder;
        }
    }
}
