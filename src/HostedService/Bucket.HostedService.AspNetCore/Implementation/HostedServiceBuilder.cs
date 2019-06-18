
using Bucket.HostedService.AspNetCore.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.HostedService.AspNetCore.Implementation
{
    public class HostedServiceBuilder : IHostedServiceBuilder
    {
        public IServiceCollection Services { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public HostedServiceBuilder(IServiceCollection services, IConfiguration configurationRoot)
        {
            Configuration = configurationRoot;
            Services = services;

            Services.AddSingleton<IBucketAgentStartup, AspNetCoreHostedService>();
            Services.AddHostedService<InstrumentationHostedService>();
        }
    }
}
