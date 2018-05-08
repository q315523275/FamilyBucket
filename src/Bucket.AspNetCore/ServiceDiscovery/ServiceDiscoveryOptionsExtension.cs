using Bucket.ServiceDiscovery.Consul;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.AspNetCore.ServiceDiscovery
{
    public static class ServiceDiscoveryOptionsExtension
    {
        public static ServiceDiscoveryOptions UseConsul(this ServiceDiscoveryOptions options, Action<ConsulServiceDiscoveryConfiguration> configure)
        {

            if (configure == null) throw new ArgumentNullException(nameof(configure));

            options.RegisterExtension(new ServiceDiscoveryConsulOptionsExtension(configure));

            return options;
        }
    }
}
