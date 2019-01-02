using Bucket.Listener.Abstractions;
using Bucket.Listener.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Bucket.Listener.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加应用监听
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddListener(this IServiceCollection services, Action<BucketListenerBuilder> action)
        {
            var service = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)service.ImplementationInstance;
            var build = new BucketListenerBuilder(services, configuration);
            action(build);
            return services;
        }
    }
}
