
using Microsoft.Extensions.Configuration;
using System;
namespace Bucket.ServiceDiscovery.Consul
{
    public static class ServiceDiscoveryOptionsExtension
    {
        public static ServiceDiscoveryOptions UseConsul(this ServiceDiscoveryOptions options, Action<ConsulServiceDiscoveryConfiguration> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var setting = new ConsulServiceDiscoveryConfiguration();
            configure.Invoke(setting);

            options.RegisterExtension(new ServiceDiscoveryConsulOptionsExtension(setting));

            return options;
        }

        public static ServiceDiscoveryOptions UseConsul(this ServiceDiscoveryOptions options, IConfiguration configuration)
        {

            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var setting = new ConsulServiceDiscoveryConfiguration();

            configuration.GetSection("ServiceDiscovery").GetSection("Consul").Bind(setting);

            options.RegisterExtension(new ServiceDiscoveryConsulOptionsExtension(setting));

            return options;
        }
    }
}
