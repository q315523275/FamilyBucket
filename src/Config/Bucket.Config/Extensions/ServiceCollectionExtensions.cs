
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
        private static IServiceCollection AddConfigService(this IServiceCollection services, ConfigOptions setting)
        {
            if (setting == null) throw new ArgumentNullException(nameof(setting));

            if (string.IsNullOrWhiteSpace(setting.ServerUrl) && string.IsNullOrWhiteSpace(setting.ServiceName))
                throw new ArgumentNullException(nameof(setting));

            services.AddSingleton(setting);
            services.AddSingleton<IConfig, DefaultConfig>();
            services.AddSingleton<IDataListener, RedisDataListener>();
            services.AddSingleton<IDataRepository, HttpDataRepository>();
            services.AddSingleton<IHttpUrlRepository, HttpUrlRepository>();
            services.AddSingleton<ILocalDataRepository, LocalDataRepository>();
            services.AddHostedService<ConfigurationPoller>();
            return services;
        }
    }
}
