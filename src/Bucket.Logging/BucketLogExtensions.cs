using Bucket.EventBus.Common.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bucket.Logging
{
    public static class BucketLogExtensions
    {

        public static ILoggerFactory AddBucketLog(this ILoggerFactory factory, IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            var provider = new BucketLogProvider(eventBus);
            factory.AddProvider(provider);
            return factory;
        }
        public static ILoggerFactory AddBucketLog(this ILoggerFactory factory, IEventBus eventBus)
        {
            var provider = new BucketLogProvider(eventBus);
            factory.AddProvider(provider);
            return factory;
        }
    }
}
