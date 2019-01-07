using Bucket.Values;

namespace Bucket.ServiceDiscovery
{
    public class ServiceDiscoveryOption
    {
        public string ServiceName { get; set; }

        public string Version { get; set; }

        public string HealthCheckTemplate { get; set; }

        public string Endpoint { get; set; }

        public ServiceType ServiceType { get; set; } = ServiceType.HTTP;
    }
}
