using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Bucket.ErrorCode.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 错误码服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IBucketErrorCodeBuilder AddErrorCodeServer(this IServiceCollection services, Action<ErrorCodeSetting> configAction)
        {
            if (configAction == null) throw new ArgumentNullException(nameof(configAction));

            services.Configure(configAction);

            var service = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)service.ImplementationInstance;

            return  new BucketErrorCodeBuilder(services, configuration);
        }
        /// <summary>
        /// 错误码服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IBucketErrorCodeBuilder AddErrorCodeServer(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<ErrorCodeSetting>(configuration.GetSection("ErrorCodeService"));

            return new BucketErrorCodeBuilder(services, configuration);
        }
    }
}
