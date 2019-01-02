using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.HostedService.AspNetCore.Abstractions
{
    public interface IHostedServiceBuilder
    {
        IServiceCollection Services { get; }

        IConfiguration Configuration { get; }
    }
}
