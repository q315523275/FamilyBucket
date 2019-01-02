using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.DependencyInjection
{
    public interface IBucketBuilder
    {
        IServiceCollection Services { get; }
        IConfiguration Configuration { get; }
    }
}
