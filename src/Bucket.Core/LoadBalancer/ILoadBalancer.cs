using Bucket.Values;
using System.Threading.Tasks;

namespace Bucket.LoadBalancer
{
    public interface ILoadBalancer
    {
        Task<HostAndPort> Lease();
        void Release(HostAndPort hostAndPort);
    }
}
