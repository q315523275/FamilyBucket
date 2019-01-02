using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.SkrTrace.DependencyInjection
{
    public interface ISkrTraceBuilder
    {
        IServiceCollection Services { get; }

        IConfiguration Configuration { get; }
    }
}
