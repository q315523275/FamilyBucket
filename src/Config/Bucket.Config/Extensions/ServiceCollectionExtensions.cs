
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using Bucket.Redis;
using Microsoft.Extensions.Logging;
using Bucket.Core;

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
        public static IServiceCollection AddConfigService(this IServiceCollection services, Action<ConfigSetting> configAction)
        {
            if (configAction == null) throw new ArgumentNullException(nameof(configAction));

            var configSetting = new ConfigSetting();
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

            var configSetting = new ConfigSetting();
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
        private static IServiceCollection AddConfigService(this IServiceCollection services, ConfigSetting configSetting)
        {
            if (configSetting.UseServiceDiscovery)
            {
                services.AddSingleton(configSetting);
                services.AddSingleton<RemoteConfigRepository>();
            }
            else
            {
                services.AddSingleton(sp => new RemoteConfigRepository(
                    configSetting,
                    sp.GetRequiredService<RedisClient>(),
                    null,
                    sp.GetRequiredService<ILoggerFactory>(),
                    sp.GetRequiredService<IJsonHelper>()
                ));
            }
            services.AddSingleton<IConfig, DefaultConfig>();
            return services;
        }
    }
}
