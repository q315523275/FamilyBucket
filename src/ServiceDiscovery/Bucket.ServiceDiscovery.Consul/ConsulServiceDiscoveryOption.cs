using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.ServiceDiscovery.Consul
{
    public class ConsulServiceDiscoveryOption
    {
        public string ServiceName { get; set; }

        public string Version { get; set; }

        public ConsulServiceDiscoveryConfiguration Consul { get; set; }

        public string HealthCheckTemplate { get; set; }

        public string[] Endpoints { get; set; }
    }
}
