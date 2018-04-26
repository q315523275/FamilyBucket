using Bucket.AspNetCore.Middleware;
using Bucket.AspNetCore.Middleware.Error;
using Bucket.AspNetCore.Middleware.Tracer;
using Microsoft.AspNetCore.Builder;

namespace Bucket.AspNetCore.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorLog(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorLogMiddleware>();
        }
        public static IApplicationBuilder UseTracer(this IApplicationBuilder builder, TracerOptions tracerOptions)
        {
            return builder.UseMiddleware<TracerMiddleware>(tracerOptions);
        }
    }
}
