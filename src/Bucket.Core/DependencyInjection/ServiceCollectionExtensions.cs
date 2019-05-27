using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Bucket.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加全家桶服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AddFamilyBucket(this IServiceCollection services, Action<BucketBuilder> builder)
        {
            var service = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)service.ImplementationInstance;
            var bucketBuild = new BucketBuilder(services, configuration);
            builder(bucketBuild);
            return services;
        }
    }
}
