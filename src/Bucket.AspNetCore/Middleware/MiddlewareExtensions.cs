using Bucket.AspNetCore.Middleware.Error;
using Microsoft.AspNetCore.Builder;

namespace Bucket.AspNetCore.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
