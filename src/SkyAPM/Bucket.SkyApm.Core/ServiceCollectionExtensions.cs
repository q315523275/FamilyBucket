using Microsoft.Extensions.DependencyInjection;

namespace Bucket.SkyApm
{
    public static class ServiceCollectionExtensions
    {
        public static SkyApmExtensions AddSkyApmExtensions(this IServiceCollection services)
        {
            return new SkyApmExtensions(services);
        }
    }
}
