using StackExchange.Redis;
using System.Collections.Generic;

namespace Bucket.Caching.StackExchangeRedis.Abstractions
{
    public interface IRedisDatabaseProvider
    {
        string DbProviderName { get; }
        IDatabase GetDatabase();
        IEnumerable<IServer> GetServerList();
    }
}
