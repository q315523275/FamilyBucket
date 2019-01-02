using Bucket.Listener;
using Bucket.Listener.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Authorize.Listener
{
    public static class ServiceCollectionExtensions
    {
        public static IBucketListenerBuilder AddAuthorize(this IBucketListenerBuilder builder)
        {
            builder.Services.AddSingleton<IBucketListener, BucketAuthorizeListener>();
            return builder;
        }
    }
}
