using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Bucket.ErrorCode
{
    public interface IBucketErrorCodeBuilder
    {
        IServiceCollection Services { get; }

        IConfiguration Configuration { get; }
    }
}
