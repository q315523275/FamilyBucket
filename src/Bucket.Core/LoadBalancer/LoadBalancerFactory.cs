using Bucket.ServiceDiscovery;
using System;
using System.Threading.Tasks;

namespace Bucket.LoadBalancer
{
    public class LoadBalancerFactory : ILoadBalancerFactory
    {
        private readonly IServiceDiscovery _serviceProvider;
        public LoadBalancerFactory(IServiceDiscovery serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<ILoadBalancer> Get(string serviceName, string loadBalancer = "RoundRobin")
        {
            switch (loadBalancer)
            {
                case "RoundRobin":
                    return new RoundRobin(async () => await _serviceProvider.FindServiceInstancesAsync(serviceName), serviceName);
                case "LeastConnection":
                    return new LeastConnection(async () => await _serviceProvider.FindServiceInstancesAsync(serviceName), serviceName);
                default:
                    return new NoLoadBalancer(await _serviceProvider.FindServiceInstancesAsync(serviceName));
            }
        }
    }
}
