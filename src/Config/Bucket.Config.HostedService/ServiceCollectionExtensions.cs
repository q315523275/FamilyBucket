
using Bucket.HostedService.AspNetCore.Abstractions;
using Bucket.HostedService;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Config.HostedService
{
    public static class ServiceCollectionExtensions
    {
        public static IHostedServiceBuilder AddConfig(this IHostedServiceBuilder builder)
        {
            builder.Services.AddSingleton<IExecutionService, BucketConfigHostedService>();
            return builder;
        }
    }
}
