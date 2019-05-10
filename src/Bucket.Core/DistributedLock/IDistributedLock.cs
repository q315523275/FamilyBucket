using System;
using System.Threading.Tasks;

namespace Bucket.DistributedLock
{
    public interface IDistributedLock
    {
        T TryLock<T>(string key, Func<T> func, TimeSpan? expiration = null);
        Task<T> TryLockAsync<T>(string key, Func<T> func, TimeSpan? expiration = null);
        Task<T> TryLockAsync<T>(string key, Func<Task<T>> func, TimeSpan? expiration = null);

        T TryWaitingLock<T>(string key, Func<T> func, TimeSpan? expiration = null);
        Task<T> TryWaitingLockAsync<T>(string key, Func<T> func, TimeSpan? expiration = null);
        Task<T> TryWaitingLockAsync<T>(string key, Func<Task<T>> func, TimeSpan? expiration = null);
    }
}
