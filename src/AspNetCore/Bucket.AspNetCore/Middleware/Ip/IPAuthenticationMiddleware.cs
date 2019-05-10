using Bucket.AspNetCore.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Bucket.AspNetCore.Middleware.IP
{
    public class IpAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _whiteListIps;
        private readonly ILogger _logger;

        public IpAuthenticationMiddleware(RequestDelegate next, string whiteListIps, ILoggerFactory loggerFactory)
        {
            _next = next;
            _whiteListIps = whiteListIps;
            _logger = loggerFactory.CreateLogger<IpAuthenticationMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            if (ValidateIfIpIsInWhiteList(context))
            {
                await _next.Invoke(context);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }
        }

        private bool ValidateIfIpIsInWhiteList(HttpContext context)
        {
            var remoteIp = IPAddressHelper.GetRequestIP(context);

            _logger.LogInformation($"访问 IP地址为 RemoteIP:{remoteIp}");

            string[] allowedIps = _whiteListIps.Split(';');
            if (!allowedIps.Any(ip => ip == remoteIp.ToString()))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return false;
            }
            return true;
        }
    }
}
