using Bucket.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bucket.ServiceDiscovery.Consul
{
    public class ServiceDiscoveryConsulOptionsExtension: IOptionsExtension
    {
        private readonly ConsulServiceDiscoveryConfiguration _options;
        public ServiceDiscoveryConsulOptionsExtension(ConsulServiceDiscoveryConfiguration options)
        {
            _options = options;
        }

        public void AddServices(IServiceCollection services)
        {
            services.AddSingleton<IServiceDiscovery>(f => new ConsulServiceDiscovery(_options));
        }
    }
}
