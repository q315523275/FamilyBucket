using Bucket.Listener;
using Bucket.Listener.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Config.Listener
{
    public static class ServiceCollectionExtensions
    {
        public static IBucketListenerBuilder AddConfig(this IBucketListenerBuilder builder)
        {
            builder.Services.AddSingleton<IBucketListener, BucketConfigListener>();
            return builder;
        }
    }
}
