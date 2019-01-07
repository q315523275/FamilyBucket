using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.ServiceDiscovery.Abstractions
{
    public interface IServiceDiscoveryBuilder
    {
        IServiceCollection Services { get; }
        IConfiguration Configuration { get; }
    }
}
