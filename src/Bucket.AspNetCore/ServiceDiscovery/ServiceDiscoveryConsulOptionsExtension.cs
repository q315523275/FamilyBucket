using Bucket.AspNetCore.DependencyInjection;
using Bucket.ServiceDiscovery;
using Bucket.ServiceDiscovery.Consul;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Bucket.AspNetCore.ServiceDiscovery
{
    public class ServiceDiscoveryConsulOptionsExtension: IOptionsExtension
    {
        private readonly Action<ConsulServiceDiscoveryConfiguration> _configure;
        public ServiceDiscoveryConsulOptionsExtension(Action<ConsulServiceDiscoveryConfiguration> configure)
        {
            _configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            var config = new ConsulServiceDiscoveryConfiguration();
            _configure?.Invoke(config);

            var dd = config;

            services.AddSingleton<IServiceDiscovery>(f => new ConsulServiceDiscovery(config));
        }
    }
}
