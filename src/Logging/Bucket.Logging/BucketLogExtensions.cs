using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bucket.Logging
{
    public static class BucketLogExtensions
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="app"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static ILoggerFactory AddBucketLog(this ILoggerFactory factory, IApplicationBuilder app, string projectName)
        {
            var logStore = app.ApplicationServices.GetRequiredService<ILogStore>();
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            var provider = new BucketLogProvider(logStore, httpContextAccessor, projectName);
            factory.AddProvider(provider);
            return factory;
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="logStore"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static ILoggerFactory AddBucketLog(this ILoggerFactory factory, ILogStore logStore, IHttpContextAccessor httpContextAccessor, string projectName)
        {
            var provider = new BucketLogProvider(logStore, httpContextAccessor, projectName);
            factory.AddProvider(provider);
            return factory;
        }
    }
}
