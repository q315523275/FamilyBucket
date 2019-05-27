using Bucket.Caching.StackExchangeRedis.Abstractions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Bucket.Caching.StackExchangeRedis.Implementation
{
    public class RedisDatabaseProvider : IRedisDatabaseProvider, IDisposable
    {
        private readonly string _configuration;
        private readonly int _retryCount = 3;
        ConnectionMultiplexer _connectionMultiplexer;
        bool _disposed;
        object sync_root = new object();
        public RedisDatabaseProvider(string name, string configuration)
        {
            DbProviderName = name;
            _configuration = configuration;
        }
        public string DbProviderName { get; }

        public bool IsConnected
        {
            get
            {
                return _connectionMultiplexer != null && _connectionMultiplexer.IsConnected && !_disposed;
            }
        }

        public void Dispose()
        {
            if (IsConnected)
            {
                if (_disposed) return;

                _disposed = true;

                try
                {
                    _connectionMultiplexer.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public bool TryConnect()
        {
            lock (sync_root)
            {
                // 是否需要弹性重试,Policy

                if (!IsConnected)
                    _connectionMultiplexer = ConnectionMultiplexer.Connect(_configuration);

                return IsConnected;
            }
        }

        public IDatabase GetDatabase()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No Redis connections are available to perform this action");
            }

            return _connectionMultiplexer.GetDatabase();
        }
        public IEnumerable<IServer> GetServerList()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No Redis connections are available to perform this action");
            }
            var endpoints = GetMastersServersEndpoints();
            foreach (var endpoint in endpoints)
            {
                yield return _connectionMultiplexer.GetServer(endpoint);
            }
        }
        private List<EndPoint> GetMastersServersEndpoints()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No Redis connections are available to perform this action");
            }

            var masters = new List<EndPoint>();
            foreach (var ep in _connectionMultiplexer.GetEndPoints())
            {
                var server = _connectionMultiplexer.GetServer(ep);
                if (server.IsConnected)
                {
                    // Cluster Get Master
                    if (server.ServerType == ServerType.Cluster)
                    {
                        masters.AddRange(server.ClusterConfiguration.Nodes.Where(n => !n.IsSlave).Select(n => n.EndPoint));
                        break;
                    }
                    // Single , Master-Slave
                    if (server.ServerType == ServerType.Standalone && !server.IsSlave)
                    {
                        masters.Add(ep);
                        break;
                    }
                }
            }
            return masters;
        }
    }
}
