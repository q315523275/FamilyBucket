using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.LoadBalancer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加负载均衡器
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddLoadBalancer(this IServiceCollection services)
        {
            services.AddSingleton<ILoadBalancerFactory, LoadBalancerFactory>();
            services.AddSingleton<ILoadBalancerHouse, LoadBalancerHouse>();
            return services;
        }
    }
}
