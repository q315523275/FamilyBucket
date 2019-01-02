using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Authorize
{
    public interface IAuthoriserBuilder
    {
        IServiceCollection Services { get; }

        IConfiguration Configuration { get; }
    }
}
