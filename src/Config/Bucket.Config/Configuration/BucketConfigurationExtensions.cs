using Bucket.Config;
using Bucket.Config.Configuration;

namespace Microsoft.Extensions.Configuration
{
    public static class BucketConfigurationExtensions
    {
        public static IConfigurationBuilder AddBucketConfig(this IConfigurationBuilder builder, BucketConfigOptions options)
        {
            builder.Add(new BucketConfigurationProvider(options));
            return builder;
        }
    }
}
