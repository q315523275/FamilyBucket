using Bucket.ErrorCode.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.ErrorCode.Implementation
{
    public class BucketErrorCodeBuilder : IBucketErrorCodeBuilder
    {
        public IServiceCollection Services { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public BucketErrorCodeBuilder(IServiceCollection services, IConfiguration configurationRoot)
        {
            Configuration = configurationRoot;
            Services = services;

            Services.AddSingleton<IDataRepository, HttpDataRepository>();
            Services.AddSingleton<IHttpUrlRepository, HttpUrlRepository>();
            Services.AddSingleton<ILocalDataRepository, LocalDataRepository>();
            Services.AddSingleton<IErrorCode, DefaultErrorCode>();
        }
    }
}
