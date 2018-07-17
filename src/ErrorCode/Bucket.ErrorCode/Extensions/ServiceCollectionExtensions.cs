using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bucket.ErrorCode.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 错误码服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddErrorCodeServer(this IServiceCollection services, Action<ErrorCodeSetting> configAction)
        {
            if (configAction == null) throw new ArgumentNullException(nameof(configAction));

            services.Configure(configAction);

            services.AddSingleton<RemoteStoreRepository>();
            services.AddSingleton<IErrorCode, DefaultErrorCode>();

            return services;
        }
        /// <summary>
        /// 错误码服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddErrorCodeServer(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<ErrorCodeSetting>(configuration.GetSection("ErrorCodeService"));

            services.AddSingleton<RemoteStoreRepository>();
            services.AddSingleton<IErrorCode, DefaultErrorCode>();

            return services;
        }
    }
}
