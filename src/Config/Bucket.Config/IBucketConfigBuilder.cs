using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Bucket.Config
{
    public interface IBucketConfigBuilder
    {
        IServiceCollection Services { get; }

        IConfiguration Configuration { get; }
    }
}
