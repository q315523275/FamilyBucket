using Bucket.Caching.Abstractions;
using Bucket.Caching.InMemory.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Bucket.Caching.InMemory
{
    public class DefaultInMemoryCachingProvider : ICachingProvider
    {
        private readonly IInMemoryCaching _cache;

        public DefaultInMemoryCachingProvider(string name, IEnumerable<IInMemoryCaching> cachings)
        {
            ProviderName = name;
            _cache = cachings.FirstOrDefault(it => it.ProviderName == name);
        }

        public string ProviderName { get; }

        public bool Exists(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return _cache.Exists(key);
        }

        public Task<bool> ExistsAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return Task.FromResult(_cache.Exists(key));
        }

        public T Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return _cache.Get<T>(key);
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            if (keys == null || keys.Count() == 0)
                throw new ArgumentNullException(nameof(keys));

            return _cache.GetAll<T>(keys);
        }

        public Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            if (keys == null || keys.Count() == 0)
                throw new ArgumentNullException(nameof(keys));

            return Task.FromResult(_cache.GetAll<T>(keys));
        }

        public Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return Task.FromResult(_cache.Get<T>(key));
        }

        public void Refresh<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
                throw new ArgumentNullException(nameof(cacheKey));
            if (cacheValue == null)
                throw new ArgumentNullException(nameof(cacheValue));

            this.Remove(cacheKey);
            this.Set(cacheKey, cacheValue, expiration);
        }

        public async Task RefreshAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
                throw new ArgumentNullException(nameof(cacheKey));
            if (cacheValue == null)
                throw new ArgumentNullException(nameof(cacheValue));

            await this.RemoveAsync(cacheKey);
            await this.SetAsync(cacheKey, cacheValue, expiration);
        }

        public void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            _cache.Remove(key);
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            if (keys == null || keys.Count() == 0)
                throw new ArgumentNullException(nameof(keys));

            _cache.RemoveAll(keys);
        }

        public Task RemoveAllAsync(IEnumerable<string> keys)
        {
            if (keys == null || keys.Count() == 0)
                throw new ArgumentNullException(nameof(keys));

            _cache.RemoveAll(keys);

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            _cache.Remove(key);

            return Task.CompletedTask;
        }

        public void Set<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
                throw new ArgumentNullException(nameof(cacheKey));
            if (cacheValue == null)
                throw new ArgumentNullException(nameof(cacheValue));

            _cache.Set(cacheKey, cacheValue, expiration);
        }

        public void SetAll<T>(IDictionary<string, T> values, TimeSpan expiration)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            _cache.SetAll(values, expiration);
        }

        public Task SetAllAsync<T>(IDictionary<string, T> values, TimeSpan expiration)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            _cache.SetAll(values, expiration);

            return Task.CompletedTask;
        }

        public Task SetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
                throw new ArgumentNullException(nameof(cacheKey));
            if (cacheValue == null)
                throw new ArgumentNullException(nameof(cacheValue));

            _cache.Set(cacheKey, cacheValue, expiration);

            return Task.CompletedTask;
        }

        public bool TrySet<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
                throw new ArgumentNullException(nameof(cacheKey));
            if (cacheValue == null)
                throw new ArgumentNullException(nameof(cacheValue));

            return _cache.Add(cacheKey, cacheValue, expiration);
        }

        public Task<bool> TrySetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
                throw new ArgumentNullException(nameof(cacheKey));
            if (cacheValue == null)
                throw new ArgumentNullException(nameof(cacheValue));

            return Task.FromResult(_cache.Add(cacheKey, cacheValue, expiration));
        }
    }
}
