using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Gprc
{
    public class RpcServiceDiscoveryOptions
    {
        public string ServiceName { get; set; }

        public string Version { get; set; }

        public string HealthCheckTemplate { get; set; }

        public string[] Endpoints { get; set; }
    }
}
