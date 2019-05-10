using Bucket.Caching.StackExchangeRedis.Abstractions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Bucket.Caching.StackExchangeRedis.Implementation
{
    public class RedisDatabaseProvider : IRedisDatabaseProvider
    {
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;
        private readonly string _configuration;
        public RedisDatabaseProvider(string name, string configuration)
        {
            DbProviderName = name;
            _configuration = configuration;
            _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
        }
        public string DbProviderName { get; }
        public IDatabase GetDatabase()
        {
            return _connectionMultiplexer.Value.GetDatabase();
        }
        public IEnumerable<IServer> GetServerList()
        {
            var endpoints = GetMastersServersEndpoints();
            foreach (var endpoint in endpoints)
            {
                yield return _connectionMultiplexer.Value.GetServer(endpoint);
            }
        }
        private ConnectionMultiplexer CreateConnectionMultiplexer()
        {
            return ConnectionMultiplexer.Connect(_configuration);
        }
        private List<EndPoint> GetMastersServersEndpoints()
        {
            var masters = new List<EndPoint>();
            foreach (var ep in _connectionMultiplexer.Value.GetEndPoints())
            {
                var server = _connectionMultiplexer.Value.GetServer(ep);
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
