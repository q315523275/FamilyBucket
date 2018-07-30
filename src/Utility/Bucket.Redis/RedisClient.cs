using StackExchange.Redis;
using System;
using System.Collections.Concurrent;

namespace Bucket.Redis
{
    public class RedisClient : IDisposable
    {
        private object _lock = new object();
        private ConcurrentDictionary<int, ConnectionMultiplexer> _connections;

        public RedisClient()
        {
            _connections = new ConcurrentDictionary<int, ConnectionMultiplexer>();
        }
        /// <summary>
        /// 获取ConnectionMultiplexer
        /// </summary>
        /// <param name="redisConnectionString"></param>
        /// <returns></returns>
        private ConnectionMultiplexer GetConnect(string redisConnectionString)
        {
            var hash = redisConnectionString.GetHashCode();
            return _connections.GetOrAdd(hash, p => {
                lock (_lock)
                {
                    return ConnectionMultiplexer.Connect(redisConnectionString);
                }
            });
        }
        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <param name="redisConnectionString"></param>
        /// <param name="db"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        public IDatabase GetDatabase(string redisConnectionString = null, int db = 0, object asyncState = null)
        {
            if(string.IsNullOrWhiteSpace(redisConnectionString))
                throw new ArgumentNullException(redisConnectionString);
            return GetConnect(redisConnectionString).GetDatabase(db, asyncState);
        }

        public ISubscriber GetSubscriber(string redisConnectionString)
        {
            return GetConnect(redisConnectionString).GetSubscriber();
        }

        public void Dispose()
        {
            if (_connections != null && _connections.Count > 0)
            {
                foreach (var item in _connections.Values)
                {
                    item.Close();
                }
            }
        }
    }
}
