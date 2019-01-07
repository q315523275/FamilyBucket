using Bucket.ServiceDiscovery.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.ServiceDiscovery.Implementation
{
    public class ServiceDiscoveryBuilder : IServiceDiscoveryBuilder
    {
        public ServiceDiscoveryBuilder(IServiceCollection services, IConfiguration configuration)
        {
            Services = services;
            Configuration = configuration;
        }

        public IServiceCollection Services { get; private set; }
        public IConfiguration Configuration { get; private set; }
    }
}
