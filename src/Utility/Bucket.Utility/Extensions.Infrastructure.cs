using Bucket.DependencyInjection;
using Bucket.Utility.Files;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddSingleton<IBucketFileProvider>(sp =>
            {
                var environment = sp.GetRequiredService<IHostingEnvironment>();
                return new BucketFileProvider(environment.ContentRootPath);
            });
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

        /// <summary>
        /// 使用静态HttpContext
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseStaticHttpContext(this IApplicationBuilder app)
        {
            Helpers.Web.HttpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            return app;
        }
    }
}
