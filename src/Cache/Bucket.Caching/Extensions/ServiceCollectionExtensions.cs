using System;
using System.Linq;
using Bucket.Caching.Abstractions;
using Bucket.Caching.Implementation;
using Bucket.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Bucket.Caching.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加缓存组件
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IServiceCollection AddCaching(this IServiceCollection services, Action<DefaultCachingBuilder> action)
        {
            var service = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)service.ImplementationInstance;

            services.AddSingleton<ICachingSerializer, DefaultJsonCachingSerializer>();
            services.AddSingleton<ICachingProviderFactory, DefaultCachingProviderFactory>();

            var builder = new DefaultCachingBuilder(services, configuration);
            action(builder);
            return services;
        }
        /// <summary>
        /// 添加缓存组件
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IServiceCollection AddCaching(this IBucketBuilder builder, Action<DefaultCachingBuilder> action)
        {
            return AddCaching(builder.Services, action);
        }
    }
}
