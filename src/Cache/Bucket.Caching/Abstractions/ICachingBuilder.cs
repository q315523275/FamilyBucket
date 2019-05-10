using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Caching.Abstractions
{
    public interface ICachingBuilder
    {
        IServiceCollection Services { get; }
        IConfiguration Configuration { get; }
    }
}
