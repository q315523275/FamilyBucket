using Bucket.DependencyInjection;
using Bucket.ServiceDiscovery.Abstractions;
using Bucket.ServiceDiscovery.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Bucket.ServiceDiscovery.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加服务发现
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IServiceDiscoveryBuilder AddServiceDiscovery(this IServiceCollection services, Action<IServiceDiscoveryBuilder> action)
        {
            var service = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)service.ImplementationInstance;
            var builder = new ServiceDiscoveryBuilder(services, configuration);
            action(builder);
            return builder;
        }
        /// <summary>
        /// 添加服务发现
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IServiceDiscoveryBuilder AddServiceDiscovery(this IServiceCollection services, IConfiguration configuration, Action<IServiceDiscoveryBuilder> action)
        {
            var builder = new ServiceDiscoveryBuilder(services, configuration);
            action(builder);
            return builder;
        }
        /// <summary>
        /// 添加服务发现
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IServiceDiscoveryBuilder AddServiceDiscovery(this IBucketBuilder builder, Action<IServiceDiscoveryBuilder> action)
        {
            return AddServiceDiscovery(builder.Services, action);
        }
    }
}
