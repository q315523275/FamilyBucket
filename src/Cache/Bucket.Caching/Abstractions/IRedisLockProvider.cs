using System;
using System.Threading.Tasks;

namespace Bucket.Caching.Abstractions
{
    public interface IRedisLockProvider
    {
        T TryLock<T>(string key, Func<T> func, TimeSpan? expiration = null);
        Task<T> TryLockAsync<T>(string key, Func<T> func, TimeSpan? expiration = null);
        Task<T> TryLockAsync<T>(string key, Func<Task<T>> func, TimeSpan? expiration = null);
    }
}
