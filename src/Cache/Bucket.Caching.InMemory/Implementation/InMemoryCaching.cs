using Bucket.Caching.InMemory.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bucket.Caching.InMemory.Implementation
{
    public class InMemoryCaching : IInMemoryCaching
    {
        private readonly IMemoryCache _cache;
        public InMemoryCaching(string name)
        {
            ProviderName = name;
            _cache = new MemoryCache(new MemoryCacheOptions() { });
        }

        public string ProviderName { get; }

        public bool Add<T>(string key, T value, TimeSpan? expiresIn = null)
        {
            if (Exists(key))
                return false;
            Set(key, value, expiresIn);
            return true;
        }

        public T Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (_cache.TryGetValue(key, out T _value))
                return _value;

            return default(T);
        }

        public bool Set<T>(string key, T value, TimeSpan? expiresIn = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (expiresIn.HasValue)
                _cache.Set(key, value, new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(expiresIn.Value.TotalSeconds)
                });
            else
                _cache.Set(key, value);

            return true;
        }

        public bool Exists(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return _cache.TryGetValue(key, out _);
        }


        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            var map = new Dictionary<string, T>();

            foreach (string key in keys)
                map[key] = Get<T>(key);

            return map;
        }

        public int SetAll<T>(IDictionary<string, T> values, TimeSpan? expiresIn = null)
        {
            if (values == null || values.Count == 0)
                return 0;

            var list = new List<bool>();

            foreach (var entry in values)
                list.Add(Set(entry.Key, entry.Value, expiresIn));

            return list.Count(r => r);
        }

        public bool Remove(string key)
        {
            _cache.Remove(key);
            return true;
        }

        public int RemoveAll(IEnumerable<string> keys)
        {
            if (keys == null || keys.Count() == 0)
                throw new ArgumentNullException(nameof(keys));

            int removed = 0;

            foreach (string key in keys)
            {
                if (string.IsNullOrEmpty(key))
                    continue;

                _cache.Remove(key); removed++;
            }

            return removed;
        }
    }
}
