
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace Bucket.Config.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 配置中心
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IBucketConfigBuilder AddConfigService(this IServiceCollection services, Action<ConfigOptions> configAction)
        {
            if (configAction == null) throw new ArgumentNullException(nameof(configAction));
            services.Configure(configAction);

            return AddConfigService(services);
        }
        /// <summary>
        /// 配置中心
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IBucketConfigBuilder AddConfigService(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<ConfigOptions>(configuration.GetSection("ConfigService"));

            return AddConfigService(services);
        }
        /// <summary>
        /// 配置中心
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configSetting"></param>
        /// <returns></returns>
        private static IBucketConfigBuilder AddConfigService(this IServiceCollection services)
        {
            var service = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)service.ImplementationInstance;

            return new BucketConfigBuilder(services, configuration);
        }
    }
}
