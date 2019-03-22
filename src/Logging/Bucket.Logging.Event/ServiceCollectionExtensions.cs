using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bucket.Logging.Events
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加日志消息总线传输方式
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddLogEventTransport(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerTransport, LogEventTransport>();
            services.AddSingleton<ILoggerDispatcher, AsyncQueueLoggerDispatcher>();
            services.AddHostedService<LoggerHostedService>();
            return services;
        }
        /// <summary>
        /// 添加日志消息总线传输方式
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        [Obsolete(" => AddLogEventTransport()")]
        public static IServiceCollection AddEventLog(this IServiceCollection services) => AddLogEventTransport(services);
    }
}
