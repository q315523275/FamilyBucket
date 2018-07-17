using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bucket.ServiceDiscovery.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 服务发现
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddServiceDiscovery(this IServiceCollection services, Action<ServiceDiscoveryOptions> configAction)
        {
            if (configAction == null) throw new ArgumentNullException(nameof(configAction));

            var options = new ServiceDiscoveryOptions();
            configAction?.Invoke(options);

            foreach (var serviceExtension in options.Extensions)
                serviceExtension.AddServices(services);

            return services;
        }
    }
}
