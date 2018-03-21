using Bucket.EventBus.Common.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bucket.Logging
{
    public static class BucketLogExtensions
    {

        public static ILoggerFactory AddBucketLog(this ILoggerFactory factory, IApplicationBuilder app, string projectName)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            var provider = new BucketLogProvider(eventBus, httpContextAccessor, projectName);
            factory.AddProvider(provider);
            return factory;
        }
        public static ILoggerFactory AddBucketLog(this ILoggerFactory factory, IEventBus eventBus, IHttpContextAccessor httpContextAccessor, string projectName)
        {
            var provider = new BucketLogProvider(eventBus, httpContextAccessor, projectName);
            factory.AddProvider(provider);
            return factory;
        }
    }
}
