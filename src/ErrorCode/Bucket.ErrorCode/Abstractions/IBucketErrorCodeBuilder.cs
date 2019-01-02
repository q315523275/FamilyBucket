using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Bucket.ErrorCode.Abstractions
{
    public interface IBucketErrorCodeBuilder
    {
        IServiceCollection Services { get; }

        IConfiguration Configuration { get; }
    }
}
