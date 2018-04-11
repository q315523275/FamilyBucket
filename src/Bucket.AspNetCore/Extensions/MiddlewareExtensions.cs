using Bucket.AspNetCore.Middleware.Error;
using Microsoft.AspNetCore.Builder;

namespace Bucket.AspNetCore.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorLog(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorLogMiddleware>();
        }
    }
}
