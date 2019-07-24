using Bucket.Caching.Internal;
using System;
using System.Threading.Tasks;

namespace Bucket.Caching.Abstractions
{
    public interface IRedisCachingProvider : IRedisHashCachingProvider, IRedisLockProvider
    {
        string ProviderName { get; }


        double Incr(string key, double incrVal);
        Task<double> IncrByAsync(string key, double incrVal);
        double Decr(string key, double incrVal);
        Task<double> DecrByAsync(string key, double incrVal);



        void Expire(string key, TimeSpan timeout);
        Task ExpireAsync(string key, TimeSpan timeout);
        void Expire(string key, ExpirationMode mode, TimeSpan timeout);
        Task ExpireAsync(string key, ExpirationMode mode, TimeSpan timeout);
    }
}
