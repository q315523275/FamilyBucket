using Bucket.AspNetCore.Middleware.Error;
using Bucket.AspNetCore.Middleware.IP;
using Microsoft.AspNetCore.Builder;
using System;

namespace Bucket.AspNetCore.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorLog(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorLogMiddleware>();
        }
        /// <summary>
        /// Habilita o uso do Middleware de autenticação básica
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseIpAuthenticationMiddleware(this IApplicationBuilder app, string whiteListIps, string path = "/api")
        {
            return app.UseWhen(x => (x.Request.Path.StartsWithSegments(path, StringComparison.OrdinalIgnoreCase)),
                builder =>
                {
                    builder.UseMiddleware<IpAuthenticationMiddleware>(whiteListIps);
                });
        }
    }
}
