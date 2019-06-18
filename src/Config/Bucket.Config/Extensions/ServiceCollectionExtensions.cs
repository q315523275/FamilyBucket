
using Bucket.Config.Abstractions;
using Bucket.Config.Implementation;
using Bucket.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        public static IBucketConfigBuilder AddConfigServer(this IServiceCollection services, Action<BucketConfigOptions> configAction)
        {
            if (configAction == null)
                throw new ArgumentNullException(nameof(configAction));

            services.Configure(configAction);

            var service = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)service.ImplementationInstance;

            return new BucketConfigBuilder(services, configuration);
        }
        /// <summary>
        /// 配置中心
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IBucketConfigBuilder AddConfigServer(this IServiceCollection services, IConfiguration configuration, string section = "ConfigServer")
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<BucketConfigOptions>(configuration.GetSection(section));

            return new BucketConfigBuilder(services, configuration);
        }
        /// <summary>
        /// 添加配置中心服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configSetting"></param>
        /// <returns></returns>
        public static IBucketConfigBuilder AddConfigServer(this IBucketBuilder builder, string section = "ConfigServer")
        {
            return AddConfigServer(builder.Services, builder.Configuration, section);
        }
    }
}
