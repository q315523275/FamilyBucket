using Microsoft.Extensions.DependencyInjection;

namespace Bucket.SkyApm
{
    public class SkyApmExtensions
    {
        public IServiceCollection Services { get; }

        public SkyApmExtensions(IServiceCollection services)
        {
            Services = services;
        }
    }
}
