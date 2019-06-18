using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bucket.Caching.Abstractions
{
    public interface ICachingProvider
    {
        string ProviderName { get; }
        void Set<T>(string key, T value, TimeSpan expiration);
        Task SetAsync<T>(string key, T value, TimeSpan expiration);
        T Get<T>(string key);
        Task<T> GetAsync<T>(string key);
        void Remove(string key);
        Task RemoveAsync(string key);
        bool Exists(string key);
        Task<bool> ExistsAsync(string key);
        void Refresh<T>(string key, T value, TimeSpan expiration);
        Task RefreshAsync<T>(string key, T value, TimeSpan expiration);
        void SetAll<T>(IDictionary<string, T> values, TimeSpan expiration);
        Task SetAllAsync<T>(IDictionary<string, T> values, TimeSpan expiration);
        IDictionary<string, T> GetAll<T>(IEnumerable<string> keys);
        Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys);
        void RemoveAll(IEnumerable<string> keys);
        Task RemoveAllAsync(IEnumerable<string> keys);
        bool TrySet<T>(string key, T value, TimeSpan expiration);
        Task<bool> TrySetAsync<T>(string key, T value, TimeSpan expiration);
    }
}
