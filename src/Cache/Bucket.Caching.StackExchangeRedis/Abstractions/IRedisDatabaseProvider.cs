using StackExchange.Redis;
using System.Collections.Generic;

namespace Bucket.Caching.StackExchangeRedis.Abstractions
{
    public interface IRedisDatabaseProvider
    {
        string DbProviderName { get; }
        bool IsConnected { get; }
        bool TryConnect();
        IDatabase GetDatabase();
        IEnumerable<IServer> GetServerList();
    }
}
