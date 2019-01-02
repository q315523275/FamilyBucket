using Bucket.Listener;
using Bucket.Listener.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.ErrorCode.Listener
{
    public static class ServiceCollectionExtensions
    {
        public static IBucketListenerBuilder AddErrorCode(this IBucketListenerBuilder builder)
        {
            builder.Services.AddSingleton<IBucketListener, BucketErrorCodeListener>();
            return builder;
        }
    }
}
