using System;
using System.Collections.Generic;

namespace Bucket.Caching.InMemory.Abstractions
{
    public interface IInMemoryCaching
    {
        string ProviderName { get; }
        bool Add<T>(string key, T value, TimeSpan? expiresIn = null);
        T Get<T>(string key);
        bool Set<T>(string key, T value, TimeSpan? expiresIn = null);
        bool Exists(string key);
        int RemoveAll(IEnumerable<string> keys);
        bool Remove(string key);
        IDictionary<string, T> GetAll<T>(IEnumerable<string> keys);
        int SetAll<T>(IDictionary<string, T> values, TimeSpan? expiresIn = null);
    }
}
