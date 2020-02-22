using StackExchange.Redis;
using System;
using System.Threading.Tasks;

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
        public async Task<IDatabase> GetDatabaseAsync(string redisConnectionString = null, int db = 0, object asyncState = null)
        {
            if (string.IsNullOrWhiteSpace(redisConnectionString))
                throw new ArgumentNullException(redisConnectionString);
            var connect = await DefaultRedisPersistentConnection.GetConnectAsync(redisConnectionString);
            if (!connect.IsConnected())
            {
                connect = await DefaultRedisPersistentConnection.TryConnectAsync(redisConnectionString);
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
        public async Task<ISubscriber> GetSubscriberAsync(string redisConnectionString)
        {
            if (string.IsNullOrWhiteSpace(redisConnectionString))
                throw new ArgumentNullException(redisConnectionString);
            var connect = await DefaultRedisPersistentConnection.GetConnectAsync(redisConnectionString);
            if (!connect.IsConnected())
            {
                connect = await DefaultRedisPersistentConnection.TryConnectAsync(redisConnectionString);
            }
            return connect.GetSubscriber();
        }
        public void Dispose()
        {
            DefaultRedisPersistentConnection.Dispose();
        }
    }
}
