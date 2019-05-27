using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Authorize.Abstractions
{
    public interface IAuthoriserBuilder
    {
        IServiceCollection Services { get; }

        IConfiguration Configuration { get; }
    }
}
