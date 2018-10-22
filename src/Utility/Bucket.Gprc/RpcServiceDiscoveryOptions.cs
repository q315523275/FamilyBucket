namespace Bucket.Gprc
{
    public class RpcServiceDiscoveryOptions
    {
        public string ServiceName { get; set; }

        public string Version { get; set; }

        public string HealthCheckTemplate { get; set; }

        public string Endpoint { get; set; }

        public RpcEndpointOptions RpcEndpoint { get; set; }
    }
    public class RpcEndpointOptions
    {
        public string Address { get; set; }

        public int Port { get; set; }
    }
}
