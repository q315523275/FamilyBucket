using System;

namespace Bucket.WebApi.Hangfire
{
    /// <summary>
    /// Hangfire任务调度容器
    /// </summary>
    public static class HangfireServiceProvider
    {
        /// <summary>
        /// 容器实例
        /// </summary>
        public static IServiceProvider ServiceProvider { get; set; }
    }
}
