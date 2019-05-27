using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace Bucket.Logging
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加日志应用
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="app"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static ILoggerFactory AddBucketLog(this ILoggerFactory factory, IApplicationBuilder app, string projectName)
        {
            var loggerDispatcher = app.ApplicationServices.GetRequiredService<ILoggerDispatcher>() ??
                throw new ArgumentException("Missing Dependency", nameof(ILoggerDispatcher));

            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>() ??
                throw new ArgumentException("Missing Dependency", nameof(IHttpContextAccessor));

            factory.AddProvider(new BucketLogProvider(loggerDispatcher, httpContextAccessor, projectName));

            return factory;
        }
        /// <summary>
        /// 添加日志应用
        /// </summary>
        /// <param name="app"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static IApplicationBuilder AddBucketLog(this IApplicationBuilder app, string projectName)
        {
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>() ??
                throw new ArgumentException("Missing Dependency", nameof(ILoggerFactory));

            var loggerDispatcher = app.ApplicationServices.GetRequiredService<ILoggerDispatcher>() ??
                throw new ArgumentException("Missing Dependency", nameof(ILoggerDispatcher));

            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>() ??
                throw new ArgumentException("Missing Dependency", nameof(IHttpContextAccessor));

            loggerFactory.AddProvider(new BucketLogProvider(loggerDispatcher, httpContextAccessor, projectName));

            return app;
        }
        /// <summary>
        /// 添加日志应用
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddBucketLog(this ILoggingBuilder builder, string projectName)
        {
            builder.ClearProviders(); // 清除已实现
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSingleton<ILoggerProvider>(sp =>
            {

                var loggerDispatcher = sp.GetRequiredService<ILoggerDispatcher>() ?? throw new ArgumentException("Missing Dependency", nameof(ILoggerDispatcher));

                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>() ?? throw new ArgumentException("Missing Dependency", nameof(IHttpContextAccessor));

                return new BucketLogProvider(loggerDispatcher, httpContextAccessor, projectName);
            });
            return builder;
        }
        /// <summary>
        /// 添加日志应用
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="projectName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddBucketLog(this ILoggingBuilder builder, string projectName, Func<string, LogLevel, bool> filter)
        {
            builder.AddFilter(filter);
            builder.AddBucketLog(projectName);
            return builder;
        }
    }
}
