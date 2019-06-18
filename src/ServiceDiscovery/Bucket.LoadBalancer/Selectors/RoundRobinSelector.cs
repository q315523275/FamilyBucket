using Bucket.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.LoadBalancer.Selectors
{
    public class RoundRobinSelector : ILoadBalancer
    {
        private readonly Func<Task<IList<ServiceInformation>>> _services;
        private readonly string _serviceName;
        private int _last;
        public RoundRobinSelector(Func<Task<IList<ServiceInformation>>> services, string serviceName)
        {
            _services = services;
            _serviceName = serviceName;
        }

        public async Task<HostAndPort> Lease()
        {
            var services = await _services.Invoke();

            if (services == null)
                throw new ArgumentNullException($"{_serviceName}");

            if (!services.Any())
                throw new ArgumentNullException($"{_serviceName}");

            Interlocked.Increment(ref _last);

            if (_last >= services.Count)
            {
                Interlocked.Exchange(ref _last, 0);
            }

            return services[_last].HostAndPort;
        }

        public void Release(HostAndPort hostAndPort)
        {
        }
    }
}
