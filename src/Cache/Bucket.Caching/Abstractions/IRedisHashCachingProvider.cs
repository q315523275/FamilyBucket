using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bucket.Caching.Abstractions
{
    public interface IRedisHashCachingProvider
    {
        bool HMSet(string key, Dictionary<string, string> vals, TimeSpan? expiration = null);
        bool HSet(string key, string field, string cacheValue);
        bool HExists(string key, string field);
        long HDel(string key, IList<string> fields = null);
        string HGet(string key, string field);
        Dictionary<string, string> HGetAll(string key);
        long HIncrBy(string key, string field, long val = 1);
        List<string> HKeys(string key);
        long HLen(string key);
        List<string> HVals(string key);
        Dictionary<string, string> HMGet(string key, IList<string> fields);
        Task<bool> HMSetAsync(string key, Dictionary<string, string> vals, TimeSpan? expiration = null);
        Task<bool> HSetAsync(string key, string field, string cacheValue);
        Task<bool> HExistsAsync(string key, string field);
        Task<long> HDelAsync(string key, IList<string> fields = null);
        Task<string> HGetAsync(string key, string field);
        Task<Dictionary<string, string>> HGetAllAsync(string key);
        Task<long> HIncrByAsync(string key, string field, long val = 1);
        Task<List<string>> HKeysAsync(string key);
        Task<long> HLenAsync(string key);
        Task<List<string>> HValsAsync(string key);
        Task<Dictionary<string, string>> HMGetAsync(string key, IList<string> fields);
    }
}
