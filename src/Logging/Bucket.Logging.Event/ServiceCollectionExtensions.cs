using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Logging.Events
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加日志记录总线推送
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEventLog(this IServiceCollection services)
        {
            services.AddSingleton<ILogStore, LogStore>();
            return services;
        }
    }
}
