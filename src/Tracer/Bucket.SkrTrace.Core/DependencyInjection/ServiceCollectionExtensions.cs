using Bucket.SkrTrace.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Bucket.SkrTrace.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ISkrTraceBuilder AddSkrTrace(this IServiceCollection services)
        {
            var service = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)service.ImplementationInstance;
            return new SkrTraceBuilder(services, configuration);
        }

        public static ISkrTraceBuilder AddSkrTrace(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SkrTraceOptions>(configuration.GetSection("SkrTrace"));
            return new SkrTraceBuilder(services, configuration);
        }
    }
}
