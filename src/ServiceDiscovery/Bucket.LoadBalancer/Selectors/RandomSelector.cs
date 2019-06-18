using Bucket.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bucket.LoadBalancer.Selectors
{
    public class RandomSelector : ILoadBalancer
    {
        private readonly Func<Task<IList<ServiceInformation>>> _services;
        private readonly string _serviceName;
        private readonly Func<int, int, int> _generate;
        private readonly Random _random;
        public RandomSelector(Func<Task<IList<ServiceInformation>>> services, string serviceName)
        {
            _services = services;
            _serviceName = serviceName;
            _random = new Random();
            _generate = (min, max) => _random.Next(min, max);
        }

        public async Task<HostAndPort> Lease()
        {
            var services = await _services.Invoke();

            if (services == null)
                throw new ArgumentNullException($"{_serviceName}");

            if (!services.Any())
                throw new ArgumentNullException($"{_serviceName}");

            var index = _generate(0, services.Count());

            return services[index].HostAndPort;
        }

        public void Release(HostAndPort hostAndPort)
        {
        }
    }
}
