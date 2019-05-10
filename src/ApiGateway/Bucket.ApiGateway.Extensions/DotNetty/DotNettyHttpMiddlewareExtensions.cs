using Ocelot.Middleware.Pipeline;

namespace Bucket.ApiGateway.Extensions.DotNetty
{
    public static class DotNettyHttpMiddlewareExtensions
    {
        public static IOcelotPipelineBuilder UseDotNettyHttpMiddleware(this IOcelotPipelineBuilder builder)
        {
            return builder.UseMiddleware<DotNettyHttpMiddleware>();
        }
    }
}
