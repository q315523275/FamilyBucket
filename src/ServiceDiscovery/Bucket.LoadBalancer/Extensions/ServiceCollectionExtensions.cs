using Bucket.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddSingleton<ILoadBalancerManager, LoadBalancerManager>();
            return services;
        }
        /// <summary>
        /// 添加负载均衡器
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddLoadBalancer(this IBucketBuilder builder)
        {
            return AddLoadBalancer(builder.Services);
        }
    }
}
