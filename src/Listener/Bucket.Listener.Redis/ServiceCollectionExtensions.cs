using Bucket.Listener.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Listener.Redis
{
    public static class ServiceCollectionExtensions
    {
        public static IBucketListenerBuilder UseRedis(this IBucketListenerBuilder builder)
        {
            builder.Services.Configure<RedisListenerOptions>(builder.Configuration.GetSection("BucketListener:Redis"));
            builder.Services.AddSingleton<IListenerAgentStartup, RedisListenerAgentStartup>();
            return builder;
        }
    }
}
