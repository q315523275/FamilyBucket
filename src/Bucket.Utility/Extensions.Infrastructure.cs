using System;
using Microsoft.Extensions.DependencyInjection;
using Bucket.Utility.Files.Paths;
using Bucket.Utility.Files;
using Bucket.Utility.Randoms;
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
            services.AddSingleton<IBasePath, DefaultBasePath>();
            services.AddSingleton<IPathGenerator, DefaultPathGenerator>();
            services.AddSingleton<IFileStore, DefaultFileStore>();
            // 添加随机ID
            services.AddSingleton<IRandomGenerator, GuidRandomGenerator>();
            // 值设置
            Bucket.Utility.Helpers.Web.HttpContextAccessor = services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>();
            Bucket.Utility.Helpers.Web.Environment = services.BuildServiceProvider().GetRequiredService<IHostingEnvironment>();
            return services;
        }
    }
}
