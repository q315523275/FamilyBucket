using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.EventBus.Abstractions
{
    public interface IEventBusBuilder
    {
        IServiceCollection Services { get; }
        IConfiguration Configuration { get; }
    }
}
