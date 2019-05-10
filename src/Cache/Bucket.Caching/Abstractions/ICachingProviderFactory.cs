namespace Bucket.Caching.Abstractions
{
    public interface ICachingProviderFactory
    {
        ICachingProvider GetCachingProvider(string name);
        IRedisCachingProvider GetRedisProvider(string name);
    }
}
