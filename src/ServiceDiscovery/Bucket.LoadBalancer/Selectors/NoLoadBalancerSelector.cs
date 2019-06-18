using Bucket.Values;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bucket.LoadBalancer.Selectors
{
    public class NoLoadBalancerSelector : ILoadBalancer
    {
        private readonly IList<ServiceInformation> _services;

        public NoLoadBalancerSelector(IList<ServiceInformation> services)
        {
            _services = services;
        }

        public async Task<HostAndPort> Lease()
        {
            var service = await Task.FromResult(_services.FirstOrDefault());
            return service.HostAndPort;
        }

        public void Release(HostAndPort hostAndPort)
        {
        }
    }
}
