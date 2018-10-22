using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bucket.Tracing.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetUserIp(this HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            var list = new[] { "127.0.0.1", "::1" };
            var result = httpContext?.Request?.Headers["X-Forwarded-For"].FirstOrDefault()?.ToString();
            if (string.IsNullOrEmpty(result))
                result = httpContext?.Connection?.RemoteIpAddress?.ToString();
            return result;
        }
        public static string GetUserId(this HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            var result = string.Empty;
            if (httpContext.User.HasClaim(it => it.Type == "Uid"))
                result = httpContext.User.Claims.FirstOrDefault(c => c.Type == "Uid")?.Value;
            return result;
        }
    }
}
