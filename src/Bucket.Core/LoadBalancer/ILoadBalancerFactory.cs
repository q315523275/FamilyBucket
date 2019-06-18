using System.Threading.Tasks;

namespace Bucket.LoadBalancer
{
    public interface ILoadBalancerFactory
    {
        Task<ILoadBalancer> Get(string serviceName, LoadBalancerMode loadBalancer = LoadBalancerMode.RoundRobin);
    }
}
