using Bucket.Config.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Config.Implementation
{
    public class BucketConfigBuilder : IBucketConfigBuilder
    {
        public IServiceCollection Services { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public BucketConfigBuilder(IServiceCollection services, IConfiguration configurationRoot)
        {
            Configuration = configurationRoot;
            Services = services;

            Services.AddSingleton<IConfig, DefaultConfig>();
            Services.AddSingleton<IDataRepository, HttpDataRepository>();
            Services.AddSingleton<IHttpUrlRepository, HttpUrlRepository>();
            Services.AddSingleton<ILocalDataRepository, LocalDataRepository>();
        }
    }
}
