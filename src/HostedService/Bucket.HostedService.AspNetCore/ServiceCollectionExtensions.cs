using Bucket.HostedService.AspNetCore.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Bucket.HostedService.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加应用服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddBucketHostedService(this IServiceCollection services, Action<HostedServiceBuilder> action)
        {
            var service = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)service.ImplementationInstance;
            var builder = new HostedServiceBuilder(services, configuration);
            action(builder);
            return services;
        }
    }
}
