using Bucket.Listener.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Listener.Implementation
{
    public class BucketListenerBuilder: IBucketListenerBuilder
    {
        public IServiceCollection Services { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public BucketListenerBuilder(IServiceCollection services, IConfiguration configurationRoot)
        {
            Configuration = configurationRoot;
            Services = services;

            Services.AddSingleton<IListenerAgentStartup, BucketListenerAgentStartup>();
            Services.AddSingleton<IExtractCommand, ExtractCommand>();
            Services.AddHostedService<InstrumentationHostedService>();
        }
    }
}
