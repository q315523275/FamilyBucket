
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Bucket.Redis
{
    /// <summary>
    /// redis连接管理
    /// </summary>
    public static class DefaultRedisPersistentConnection
    {
        private static ConcurrentDictionary<int, ConnectionMultiplexer> _connections = new ConcurrentDictionary<int, ConnectionMultiplexer>();
        private static bool _disposed = false;
        private static readonly object sync_root = new object();

        public static bool IsConnected(this ConnectionMultiplexer connect)
        {
            return connect != null && connect.IsConnected;
        }
        public static void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                if (_connections != null && _connections.Count > 0)
                {
                    foreach (var item in _connections.Values)
                    {
                        if (item.IsConnected)
                            item.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }
        public static ConnectionMultiplexer GetConnect(string redisConnectionString)
        {
            var hash = redisConnectionString.GetHashCode();
            if (_connections.ContainsKey(hash))
                return _connections[hash];
            else
            {
                lock (sync_root)
                {
                    if (!_connections.ContainsKey(hash))
                    {
                        _connections.TryAdd(hash, TryConnect(redisConnectionString));
                    }
                    return _connections[hash];
                }
            }
        }
        public static async Task<ConnectionMultiplexer> GetConnectAsync(string redisConnectionString)
        {
            var hash = redisConnectionString.GetHashCode();
            if (_connections.ContainsKey(hash))
                return _connections[hash];
            else
            {
                if (!_connections.ContainsKey(hash))
                {
                    var connect = await TryConnectAsync(redisConnectionString);
                    _connections.TryAdd(hash, connect);
                }
                return _connections[hash];
            }
        }
        public static ConnectionMultiplexer TryConnect(string redisConnectionString)
        {
            var connect = ConnectionMultiplexer.Connect(redisConnectionString);
            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ConfigurationChanged += MuxerConfigurationChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            return connect;
        }
        public static async Task<ConnectionMultiplexer> TryConnectAsync(string redisConnectionString)
        {
            var connect = await ConnectionMultiplexer.ConnectAsync(redisConnectionString);
            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ConfigurationChanged += MuxerConfigurationChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            return connect;
        }

        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            if (_disposed) return;
            Console.WriteLine("Redis:Endpoint failed: " + e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)));
        }
        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            if (_disposed) return;
            Console.WriteLine("Redis:Configuration changed:" + e.EndPoint);
        }
        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            if (_disposed) return;
            Console.WriteLine("Redis:ConnectionRestored:" + e.EndPoint);
        }
        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            if (_disposed) return;
            Console.WriteLine("Redis:HashSlotMoved:NewEndPoint" + e.NewEndPoint + ",OldEndPoint" + e.OldEndPoint);
        }
    }
}
