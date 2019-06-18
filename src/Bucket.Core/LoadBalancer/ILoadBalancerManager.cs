using System.Threading.Tasks;

namespace Bucket.LoadBalancer
{
    public interface ILoadBalancerManager
    {
        Task<ILoadBalancer> Get(string serviceName, LoadBalancerMode loadBalancer = LoadBalancerMode.RoundRobin);
    }
}
