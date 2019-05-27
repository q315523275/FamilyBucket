using Bucket.DependencyInjection;
using Bucket.ErrorCode.Abstractions;
using Bucket.ErrorCode.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Bucket.ErrorCode.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加错误码服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IBucketErrorCodeBuilder AddErrorCodeServer(this IServiceCollection services, Action<ErrorCodeSetting> configAction)
        {
            if (configAction == null) throw new ArgumentNullException(nameof(configAction));

            services.Configure(configAction);

            var service = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)service.ImplementationInstance;

            return new BucketErrorCodeBuilder(services, configuration);
        }
        /// <summary>
        /// 添加错误码服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static IBucketErrorCodeBuilder AddErrorCodeServer(this IServiceCollection services, IConfiguration configuration, string section = "ErrorCodeServer")
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<ErrorCodeSetting>(configuration.GetSection(section));

            return new BucketErrorCodeBuilder(services, configuration);
        }
        /// <summary>
        /// 添加错误码服务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static IBucketErrorCodeBuilder AddErrorCodeServer(this IBucketBuilder builder, string section = "ErrorCodeServer")
        {
            return AddErrorCodeServer(builder.Services, builder.Configuration, section);
        }
    }
}
