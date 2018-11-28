using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Bucket.Config.Abstractions
{
    public interface IBucketConfigBuilder
    {
        IServiceCollection Services { get; }

        IConfiguration Configuration { get; }
    }
}
