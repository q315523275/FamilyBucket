using Bucket.LoadBalancer.Selectors;
using Bucket.ServiceDiscovery;
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

        public async Task<ILoadBalancer> Get(string serviceName, LoadBalancerMode loadBalancer = LoadBalancerMode.RoundRobin)
        {
            switch (loadBalancer)
            {
                case LoadBalancerMode.Random:
                    return new RandomSelector(async () => await _serviceProvider.FindServiceInstancesAsync(serviceName), serviceName);
                case LoadBalancerMode.RoundRobin:
                    return new RoundRobinSelector(async () => await _serviceProvider.FindServiceInstancesAsync(serviceName), serviceName);
                case LoadBalancerMode.LeastConnection:
                    return new LeastConnectionSelector(async () => await _serviceProvider.FindServiceInstancesAsync(serviceName), serviceName);
                default:
                    return new NoLoadBalancerSelector(await _serviceProvider.FindServiceInstancesAsync(serviceName));
            }
        }
    }
}
