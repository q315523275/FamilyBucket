using Bucket.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bucket.LoadBalancer
{
    public class RoundRobin : ILoadBalancer
    {
        private readonly Func<Task<IList<ServiceInformation>>> _services;
        private readonly string _serviceName;
        private int _last;

        public RoundRobin(Func<Task<IList<ServiceInformation>>> services, string serviceName)
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


            if (_last >= services.Count)
            {
                _last = 0;
            }

            var next = await Task.FromResult(services[_last]);
            _last++;
            return next.HostAndPort;
        }

        public void Release(HostAndPort hostAndPort)
        {
        }
    }
}
