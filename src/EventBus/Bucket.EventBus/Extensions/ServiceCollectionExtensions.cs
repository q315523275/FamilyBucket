using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.EventBus.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 事件驱动
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddEventBus(this IServiceCollection services, Action<EventBusOptions> configAction)
        {
            if (configAction == null) throw new ArgumentNullException(nameof(configAction));

            var options = new EventBusOptions();
            configAction?.Invoke(options);

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            foreach (var serviceExtension in options.Extensions)
                serviceExtension.AddServices(services);

            return services;
        }
    }
}
