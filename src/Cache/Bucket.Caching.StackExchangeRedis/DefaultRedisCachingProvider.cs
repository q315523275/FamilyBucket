using Bucket.Caching.Abstractions;
using Bucket.Caching.StackExchangeRedis.Abstractions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bucket.Caching.StackExchangeRedis
{
    public class DefaultRedisCachingProvider : ICachingProvider
    {
        private IDatabase _cache
        {
            get
            {
                if (!_dbProvider.IsConnected)
                {
                    _dbProvider.TryConnect();
                }
                return _dbProvider.GetDatabase();
            }
        }
        private IEnumerable<IServer> _servers
        {
            get
            {
                if (!_dbProvider.IsConnected)
                {
                    _dbProvider.TryConnect();
                }
                return _dbProvider.GetServerList();
            }
        }
        private readonly IRedisDatabaseProvider _dbProvider;
        private readonly ICachingSerializer _serializer;
        public string ProviderName { get; }
        public DefaultRedisCachingProvider(string name, IEnumerable<IRedisDatabaseProvider> dbProviders, ICachingSerializer serializer)
        {
            ProviderName = name;
            _dbProvider = dbProviders.FirstOrDefault(x => x.DbProviderName.Equals(name));
            _serializer = serializer;
        }

        public bool Exists(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return _cache.KeyExists(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return await _cache.KeyExistsAsync(key);
        }

        public T Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var result = _cache.StringGet(key);
            if (result.HasValue && !result.IsNull)
            {
                return _serializer.Deserialize<T>(result);
            }
            return default;
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var keyArray = keys.ToArray();
            var values = _cache.StringGet(keyArray.Select(k => (RedisKey)k).ToArray());

            var result = new Dictionary<string, T>();
            for (int i = 0; i < keyArray.Length; i++)
            {
                var cachedValue = values[i];
                if (cachedValue.HasValue && !cachedValue.IsNull)
                    result.Add(keyArray[i], _serializer.Deserialize<T>(cachedValue));
                else
                    result.Add(keyArray[i], default);
            }

            return result;
        }

        public async Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var keyArray = keys.ToArray();
            var values = await _cache.StringGetAsync(keyArray.Select(k => (RedisKey)k).ToArray());

            var result = new Dictionary<string, T>();
            for (int i = 0; i < keyArray.Length; i++)
            {
                var cachedValue = values[i];
                if (cachedValue.HasValue && !cachedValue.IsNull)
                    result.Add(keyArray[i], _serializer.Deserialize<T>(cachedValue));
                else
                    result.Add(keyArray[i], default);
            }

            return result;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var result = await _cache.StringGetAsync(key);
            if (result.HasValue && !result.IsNull)
            {
                return _serializer.Deserialize<T>(result);
            }
            return default;
        }

        public void Refresh<T>(string key, T value, TimeSpan expiration)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            Remove(key);
            Set(key, value, expiration);
        }

        public async Task RefreshAsync<T>(string key, T value, TimeSpan expiration)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            await RemoveAsync(key);
            await SetAsync(key, value, expiration);
        }

        public void Remove(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _cache.KeyDelete(key);
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var redisKeys = keys.Where(k => !string.IsNullOrEmpty(k)).Select(k => (RedisKey)k).ToArray();
            if (redisKeys.Length > 0)
                _cache.KeyDelete(redisKeys);
        }

        public async Task RemoveAllAsync(IEnumerable<string> keys)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var redisKeys = keys.Where(k => !string.IsNullOrEmpty(k)).Select(k => (RedisKey)k).ToArray();
            if (redisKeys.Length > 0)
                await _cache.KeyDeleteAsync(redisKeys);
        }

        public async Task RemoveAsync(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            await _cache.KeyDeleteAsync(key);
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _cache.StringSet(key, _serializer.Serialize(value), expiration);
        }

        public void SetAll<T>(IDictionary<string, T> values, TimeSpan expiration)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var batch = _cache.CreateBatch();

            foreach (var item in values)
                batch.StringSetAsync(item.Key, _serializer.Serialize(item.Value), expiration);

            batch.Execute();
        }

        public async Task SetAllAsync<T>(IDictionary<string, T> values, TimeSpan expiration)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var tasks = new List<Task>();

            foreach (var item in values)
                tasks.Add(SetAsync(item.Key, item.Value, expiration));

            await Task.WhenAll(tasks);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            await _cache.StringSetAsync(key, _serializer.Serialize(value), expiration);
        }

        public bool TrySet<T>(string key, T value, TimeSpan expiration)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return _cache.StringSet(key, _serializer.Serialize(value), expiration, When.NotExists);
        }

        public async Task<bool> TrySetAsync<T>(string key, T value, TimeSpan expiration)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return await _cache.StringSetAsync(key, _serializer.Serialize(value), expiration, When.NotExists);
        }
    }
}
