using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bucket.LoadBalancer
{
    public class LoadBalancerManager : ILoadBalancerManager
    {
        private readonly ILoadBalancerFactory _factory;
        private readonly ConcurrentDictionary<string, ILoadBalancer> _loadBalancers;

        public LoadBalancerManager(ILoadBalancerFactory factory)
        {
            _factory = factory;
            _loadBalancers = new ConcurrentDictionary<string, ILoadBalancer>();
        }

        public async Task<ILoadBalancer> Get(string serviceName, LoadBalancerMode _loadBalancer = LoadBalancerMode.RoundRobin)
        {
            try
            {
                if (_loadBalancers.TryGetValue(serviceName, out var loadBalancer))
                {
                    loadBalancer = _loadBalancers[serviceName];
                    if (serviceName != loadBalancer.GetType().Name)
                    {
                        loadBalancer = await _factory.Get(serviceName, _loadBalancer);
                        AddLoadBalancer(serviceName, loadBalancer);
                    }
                    return loadBalancer;
                }

                loadBalancer = await _factory.Get(serviceName, _loadBalancer);
                AddLoadBalancer(serviceName, loadBalancer);
                return loadBalancer;
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException($"unabe to find load balancer for {serviceName} exception is {ex}");
            }
        }

        private void AddLoadBalancer(string key, ILoadBalancer loadBalancer)
        {
            _loadBalancers.AddOrUpdate(key, loadBalancer, (x, y) => loadBalancer);
        }
    }
}
