using System.Threading.Tasks;

namespace Bucket.LoadBalancer
{
    public interface ILoadBalancerHouse
    {
        Task<ILoadBalancer> Get(string serviceName, string loadBalancer = "RoundRobin");
    }
}
