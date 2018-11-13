using StackExchange.Redis;
using System;

namespace Bucket.Redis
{
    public class RedisClient : IDisposable
    {
        public IDatabase GetDatabase(string redisConnectionString = null, int db = 0, object asyncState = null)
        {
            if (string.IsNullOrWhiteSpace(redisConnectionString))
                throw new ArgumentNullException(redisConnectionString);
            var connect = DefaultRedisPersistentConnection.GetConnect(redisConnectionString);
            if (!connect.IsConnected())
            {
                connect = DefaultRedisPersistentConnection.TryConnect(redisConnectionString);
            }
            return connect.GetDatabase(db, asyncState);
        }

        public ISubscriber GetSubscriber(string redisConnectionString)
        {
            if (string.IsNullOrWhiteSpace(redisConnectionString))
                throw new ArgumentNullException(redisConnectionString);
            var connect = DefaultRedisPersistentConnection.GetConnect(redisConnectionString);
            if (!connect.IsConnected())
            {
                connect = DefaultRedisPersistentConnection.TryConnect(redisConnectionString);
            }
            return connect.GetSubscriber();
        }

        public void Dispose()
        {
            DefaultRedisPersistentConnection.Dispose();
        }
    }
}
