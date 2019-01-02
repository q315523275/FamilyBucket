using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Listener.Abstractions
{
    public interface IBucketListenerBuilder
    {
        IServiceCollection Services { get; }

        IConfiguration Configuration { get; }
    }
}
