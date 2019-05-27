using Bucket.Caching.Abstractions;
using Bucket.Caching.InMemory.Abstractions;
using Bucket.Caching.InMemory.Implementation;
using Microsoft.Extensions.DependencyInjection;
namespace Bucket.Caching.InMemory
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 使用内存缓存
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static ICachingBuilder UseInMemory(this ICachingBuilder builder, string providerName = "default")
        {
            builder.Services.AddSingleton<IInMemoryCaching>(sp =>
            {
                return new InMemoryCaching(providerName);
            });
            builder.Services.AddSingleton<ICachingProvider>(sp =>
            {
                return new DefaultInMemoryCachingProvider(providerName, sp.GetServices<IInMemoryCaching>());
            });
            return builder;
        }
    }
}
