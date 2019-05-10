using Bucket.Caching.Abstractions;
using Bucket.Caching.Internal;
using Bucket.Caching.StackExchangeRedis.Abstractions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bucket.Caching.StackExchangeRedis
{
    public class DefaultRedisProvider : IRedisCachingProvider
    {
        public string ProviderName { get; }
        private readonly IDatabase _cache;
        private readonly IEnumerable<IServer> _servers;
        private readonly IRedisDatabaseProvider _dbProvider;
        private readonly ICachingSerializer _serializer;

        public DefaultRedisProvider(string name, IEnumerable<IRedisDatabaseProvider> dbProviders, ICachingSerializer serializer)
        {
            ProviderName = name;
            _dbProvider = dbProviders.FirstOrDefault(x => x.DbProviderName.Equals(name));
            _serializer = serializer;
            _cache = _dbProvider.GetDatabase();
            _servers = _dbProvider.GetServerList();
        }

        #region PUBLIC
        public double Incr(string key, double incrVal)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (incrVal == 0)
                throw new ArgumentNullException(nameof(incrVal));
            return _cache.StringIncrement(key, incrVal);
        }

        public async Task<double> IncrByAsync(string key, double incrVal)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (incrVal == 0)
                throw new ArgumentNullException(nameof(incrVal));
            return await _cache.StringIncrementAsync(key, incrVal);
        }
        public double Decr(string key, double incrVal)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (incrVal == 0)
                throw new ArgumentNullException(nameof(incrVal));
            return _cache.StringDecrement(key, incrVal);
        }

        public async Task<double> DecrByAsync(string key, double incrVal)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (incrVal == 0)
                throw new ArgumentNullException(nameof(incrVal));
            return await _cache.StringDecrementAsync(key, incrVal);
        }

        public void Expire(string key, TimeSpan timeout)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            _cache.KeyExpire(key, DateTime.Now.AddSeconds(timeout.TotalSeconds));
        }

        public async Task ExpireAsync(string key, TimeSpan timeout)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            await _cache.KeyExpireAsync(key, DateTime.Now.AddSeconds(timeout.TotalSeconds));
        }

        public void Expire(string key, ExpirationMode mode, TimeSpan timeout)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            switch (mode)
            {
                case ExpirationMode.Absolute:
                    _cache.KeyExpire(key, DateTime.Now.AddSeconds(timeout.TotalSeconds));
                    break;
                case ExpirationMode.Default:
                    _cache.KeyExpire(key, DateTime.Now.AddSeconds(timeout.TotalSeconds));
                    break;
                case ExpirationMode.None:
                    _cache.KeyExpire(key, DateTime.Now.AddDays(365));
                    break;
                case ExpirationMode.Sliding:
                    _cache.KeyExpire(key, timeout);
                    break;
            }
        }
        public async Task ExpireAsync(string key, ExpirationMode mode, TimeSpan timeout)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            switch (mode)
            {
                case ExpirationMode.Absolute:
                    await _cache.KeyExpireAsync(key, DateTime.Now.AddSeconds(timeout.TotalSeconds));
                    break;
                case ExpirationMode.Default:
                    await _cache.KeyExpireAsync(key, DateTime.Now.AddSeconds(timeout.TotalSeconds));
                    break;
                case ExpirationMode.None:
                    await _cache.KeyExpireAsync(key, DateTime.Now.AddDays(365));
                    break;
                case ExpirationMode.Sliding:
                    await _cache.KeyExpireAsync(key, timeout);
                    break;
            }
        }
        #endregion

        #region HASH
        public long HDel(string key, IList<string> fields = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (fields != null && fields.Any())
            {
                return _cache.HashDelete(key, fields.Select(x => (RedisValue)x).ToArray());
            }
            else
            {
                var flag = _cache.KeyDelete(key);
                return flag ? 1 : 0;
            }
        }

        public async Task<long> HDelAsync(string key, IList<string> fields = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (fields != null && fields.Any())
            {
                return await _cache.HashDeleteAsync(key, fields.Select(x => (RedisValue)x).ToArray());
            }
            else
            {
                var flag = await _cache.KeyDeleteAsync(key);
                return flag ? 1 : 0;
            }
        }

        public bool HExists(string key, string field)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentNullException(nameof(field));

            return _cache.HashExists(key, field);
        }

        public async Task<bool> HExistsAsync(string key, string field)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentNullException(nameof(field));

            return await _cache.HashExistsAsync(key, field);
        }

        public string HGet(string key, string field)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentNullException(nameof(field));

            return _cache.HashGet(key, field);
        }

        public Dictionary<string, string> HGetAll(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var dict = new Dictionary<string, string>();

            var vals = _cache.HashGetAll(key);

            foreach (var item in vals)
            {
                if (!dict.ContainsKey(item.Name))
                    dict.Add(item.Name, item.Value);
            }

            return dict;
        }

        public async Task<Dictionary<string, string>> HGetAllAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var dict = new Dictionary<string, string>();

            var vals = await _cache.HashGetAllAsync(key);

            foreach (var item in vals)
            {
                if (!dict.ContainsKey(item.Name))
                    dict.Add(item.Name, item.Value);
            }

            return dict;
        }

        public async Task<string> HGetAsync(string key, string field)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentNullException(nameof(field));

            return await _cache.HashGetAsync(key, field);
        }

        public long HIncrBy(string key, string field, long val = 1)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentNullException(nameof(field));

            return _cache.HashIncrement(key, field, val);
        }

        public async Task<long> HIncrByAsync(string key, string field, long val = 1)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentNullException(nameof(field));

            return await _cache.HashIncrementAsync(key, field, val);
        }

        public List<string> HKeys(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var keys = _cache.HashKeys(key);
            return keys.Select(x => x.ToString()).ToList();
        }

        public async Task<List<string>> HKeysAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var keys = await _cache.HashKeysAsync(key);
            return keys.Select(x => x.ToString()).ToList();
        }

        public long HLen(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return _cache.HashLength(key);
        }

        public async Task<long> HLenAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return await _cache.HashLengthAsync(key);
        }

        public Dictionary<string, string> HMGet(string key, IList<string> fields)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (fields == null || fields.Count == 0)
                throw new ArgumentNullException(nameof(fields));

            var dict = new Dictionary<string, string>();

            var list = _cache.HashGet(key, fields.Select(x => (RedisValue)x).ToArray());

            for (int i = 0; i < fields.Count(); i++)
            {
                if (!dict.ContainsKey(fields[i]))
                {
                    dict.Add(fields[i], list[i]);
                }
            }

            return dict;
        }

        public async Task<Dictionary<string, string>> HMGetAsync(string key, IList<string> fields)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (fields == null || fields.Count == 0)
                throw new ArgumentNullException(nameof(fields));

            var dict = new Dictionary<string, string>();

            var list = await _cache.HashGetAsync(key, fields.Select(x => (RedisValue)x).ToArray());

            for (int i = 0; i < fields.Count(); i++)
            {
                if (!dict.ContainsKey(fields[i]))
                {
                    dict.Add(fields[i], list[i]);
                }
            }

            return dict;
        }

        public bool HMSet(string key, Dictionary<string, string> vals, TimeSpan? expiration = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (vals == null || vals.Count == 0)
                throw new ArgumentNullException(nameof(vals));

            if (expiration.HasValue)
            {
                var list = new List<HashEntry>();

                foreach (var item in vals)
                {
                    list.Add(new HashEntry(item.Key, item.Value));
                }

                _cache.HashSet(key, list.ToArray());

                var flag = _cache.KeyExpire(key, expiration);

                return flag;
            }
            else
            {
                var list = new List<HashEntry>();

                foreach (var item in vals)
                {
                    list.Add(new HashEntry(item.Key, item.Value));
                }

                _cache.HashSet(key, list.ToArray());

                return true;
            }
        }

        public async Task<bool> HMSetAsync(string key, Dictionary<string, string> vals, TimeSpan? expiration = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (vals == null || vals.Count == 0)
                throw new ArgumentNullException(nameof(vals));

            if (expiration.HasValue)
            {
                var list = new List<HashEntry>();

                foreach (var item in vals)
                {
                    list.Add(new HashEntry(item.Key, item.Value));
                }

                await _cache.HashSetAsync(key, list.ToArray());

                var flag = await _cache.KeyExpireAsync(key, expiration);

                return flag;
            }
            else
            {
                var list = new List<HashEntry>();

                foreach (var item in vals)
                {
                    list.Add(new HashEntry(item.Key, item.Value));
                }

                await _cache.HashSetAsync(key, list.ToArray());

                return true;
            }
        }

        public bool HSet(string key, string field, string cacheValue)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentNullException(nameof(field));
            return _cache.HashSet(key, field, cacheValue);
        }

        public async Task<bool> HSetAsync(string key, string field, string cacheValue)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentNullException(nameof(field));
            return await _cache.HashSetAsync(key, field, cacheValue);
        }

        public List<string> HVals(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            return _cache.HashValues(key).Select(x => x.ToString()).ToList();
        }

        public async Task<List<string>> HValsAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            return (await _cache.HashValuesAsync(key)).Select(x => x.ToString()).ToList();
        }
        #endregion

        #region LOCK
        public T TryLock<T>(string lockKey, Func<T> func, TimeSpan? expiration = null)
        {
            if (string.IsNullOrWhiteSpace(lockKey))
                throw new ArgumentNullException(nameof(lockKey));

            T result;
            if (_cache.StringIncrement(lockKey) == 1)
            {
                _cache.KeyExpire(lockKey, expiration);
                try
                {
                    result = func();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    _cache.KeyDelete(lockKey);
                }
            }
            else
            {
                return default;
            }
            return result;
        }

        public async Task<T> TryLockAsync<T>(string lockKey, Func<T> func, TimeSpan? expiration = null)
        {
            if (string.IsNullOrWhiteSpace(lockKey))
                throw new ArgumentNullException(nameof(lockKey));
            T result;
            if ((await _cache.StringIncrementAsync(lockKey)) == 1)
            {
                await _cache.KeyExpireAsync(lockKey, expiration);
                try
                {
                    result = func();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    await _cache.KeyDeleteAsync(lockKey);
                }
            }
            else
            {
                return default;
            }
            return result;
        }

        public async Task<T> TryLockAsync<T>(string lockKey, Func<Task<T>> func, TimeSpan? expiration = null)
        {
            if (string.IsNullOrWhiteSpace(lockKey))
                throw new ArgumentNullException(nameof(lockKey));
            T result;
            if ((await _cache.StringIncrementAsync(lockKey)) == 1)
            {
                await _cache.KeyExpireAsync(lockKey, expiration);
                try
                {
                    result = await func();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    await _cache.KeyDeleteAsync(lockKey);
                }
            }
            else
            {
                return default;
            }
            return result;
        }
        #endregion
    }
}