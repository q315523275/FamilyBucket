
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;

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
        public static IServiceCollection AddConfigService(this IServiceCollection services, Action<ConfigOptions> configAction)
        {
            if (configAction == null) throw new ArgumentNullException(nameof(configAction));

            var configSetting = new ConfigOptions();
            configAction.Invoke(configSetting);

            AddConfigService(services, configSetting);

            return services;
        }
        /// <summary>
        /// 配置中心
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddConfigService(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var configSetting = new ConfigOptions();
            configuration.GetSection("ConfigService").Bind(configSetting);

            AddConfigService(services, configSetting);

            return services;
        }
        /// <summary>
        /// 配置中心
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configSetting"></param>
        /// <returns></returns>
        private static IServiceCollection AddConfigService(this IServiceCollection services, ConfigOptions configSetting)
        {
            if (configSetting == null) throw new ArgumentNullException(nameof(configSetting));

            if (configSetting.UseServiceDiscovery)
            {
                services.AddSingleton<ConfigServiceLocator>();
            }
            else
            {
                services.AddSingleton(sp => new ConfigServiceLocator(configSetting, null));
            }
            services.AddSingleton(configSetting);
            services.AddSingleton<RemoteConfigRepository>();
            services.AddSingleton<IConfig, DefaultConfig>();
            return services;
        }
    }
}
