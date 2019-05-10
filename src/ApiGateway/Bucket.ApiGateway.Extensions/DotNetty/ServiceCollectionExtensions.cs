using Ocelot.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Bucket.ApiGateway.Extensions.DotNetty.NettyRequest;

namespace Bucket.ApiGateway.Extensions.DotNetty
{
    public static class ServiceCollectionExtensions
    {
        public static IOcelotBuilder AddDotNettyTransport(this IOcelotBuilder builder)
        {
            builder.Services.AddSingleton<IDotNettyRequestBuilder, DotNettyRequestBuilder>();
            return builder;
        }
    }
}
