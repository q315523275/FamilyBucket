using Microsoft.Extensions.DependencyInjection;
using Bucket.Utility.Files;
using Bucket.DependencyInjection;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Bucket.Utility
{
    /// <summary>
    /// 系统扩展 - 基础设施
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// 注册Util基础设施服务
        /// </summary>
        /// <param name="services">服务集合</param>
        public static IServiceCollection AddUtil(this IServiceCollection services)
        {
            // 添加文件操作
            services.AddSingleton<IBucketFileProvider, BucketFileProvider>();

            var httpContextAccessor = services.First(x => x.ServiceType == typeof(IHttpContextAccessor));
            Helpers.Web.HttpContextAccessor = (IHttpContextAccessor)httpContextAccessor.ImplementationInstance;

            var hostingEnvironment = services.First(x => x.ServiceType == typeof(IHostingEnvironment));
            Helpers.Web.Environment = (IHostingEnvironment)hostingEnvironment.ImplementationInstance;

            return services;
        }
        /// <summary>
        /// 注册Util基础设施服务
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AddUtil(this IBucketBuilder builder)
        {
            return AddUtil(builder.Services);
        }
    }
}
