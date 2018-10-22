using Bucket.OpenTracing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracing.Events
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加链路追踪总线推送
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEventTrace(this IServiceCollection services)
        {
            services.AddSingleton<ISpanRecorder, EventSpanRecorder>();
            return services;
        }
    }
}
