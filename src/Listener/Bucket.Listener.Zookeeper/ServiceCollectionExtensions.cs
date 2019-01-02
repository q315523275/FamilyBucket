using Bucket.Listener.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Listener.Zookeeper
{
    public static class ServiceCollectionExtensions
    {
        public static IBucketListenerBuilder UseZookeeper(this IBucketListenerBuilder builder)
        {
            builder.Services.Configure<ZookeeperListenerOptions>(builder.Configuration.GetSection("BucketListener:Zookeeper"));
            builder.Services.AddSingleton<IListenerAgentStartup, ZookeeperListenerAgentStartup>();
            builder.Services.AddSingleton<IPublishCommand, ZookeeperPublishCommand>();
            return builder;
        }
    }
}
