using Bucket.ServiceDiscovery.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.ServiceDiscovery.Consul.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceDiscoveryBuilder UseConsul(this IServiceDiscoveryBuilder builder)
        {
            builder.Services.Configure<ConsulServiceDiscoveryConfiguration>(builder.Configuration.GetSection("ServiceDiscovery:Consul"));
            builder.Services.AddSingleton<IServiceDiscovery, ConsulServiceDiscovery>();
            return builder;
        }
    }
}
