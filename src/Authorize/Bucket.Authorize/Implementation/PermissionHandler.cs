using Bucket.Authorize.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace Bucket.Authorize.Implementation
{
    /// <summary>
    /// customer permission policy handler
    /// </summary>
    public class PermissionHandler : AuthorizationHandler<JwtAuthorizationRequirement>
    {
        private readonly IAuthenticationSchemeProvider _schemes;
        private readonly IPermissionAuthoriser _permissionAuthoriser;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionHandler(IAuthenticationSchemeProvider schemes, IPermissionAuthoriser permissionAuthoriser, IHttpContextAccessor httpContextAccessor)
        {
            _schemes = schemes;
            _permissionAuthoriser = permissionAuthoriser;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, JwtAuthorizationRequirement jwtAuthorizationRequirement)
        {
            //convert AuthorizationHandlerContext to HttpContext
            var httpContext = _httpContextAccessor.HttpContext;

            var handlers = httpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            foreach (var scheme in await _schemes.GetRequestHandlerSchemesAsync())
            {
                var handler = await handlers.GetHandlerAsync(httpContext, scheme.Name) as IAuthenticationRequestHandler;
                if (handler != null && await handler.HandleRequestAsync())
                {
                    httpContext.Response.Headers.Add("error", "request cancel");
                    context.Fail();
                    return;
                }
            }
            var defaultAuthenticate = await _schemes.GetDefaultAuthenticateSchemeAsync();
            if (defaultAuthenticate != null)
            {
                var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name);
                if (result?.Principal != null)
                {
                    httpContext.User = result.Principal;
                    var ipClaim = httpContext.User.Claims.SingleOrDefault(c => c.Type == "ip");
                    if (ipClaim == null)
                    {
                        var invockResult = _permissionAuthoriser.Authorise(httpContext);
                        if (invockResult)
                        {
                            context.Succeed(jwtAuthorizationRequirement);
                        }
                        else
                        {
                            context.Fail();
                        }
                    }
                    else
                    {
                        // 由于Jwt无状态方式，所以无法控制token无效关闭锁定等情况
                        // 可以通过一些特殊情况来处理
                        // token 黑名单
                        // token ip 变更
                        // httpContext.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();
                        var ip = IPAddressHelper.GetRequestIP(httpContext); 
                        if (ipClaim.Value == ip)
                        {
                            httpContext.User = result.Principal;
                            var invockResult = _permissionAuthoriser.Authorise(httpContext);
                            if (invockResult)
                            {
                                context.Succeed(jwtAuthorizationRequirement);
                            }
                            else
                            {
                                context.Fail();
                            }
                        }
                        else
                        {
                            httpContext.Response.Headers.Add("error", "token ip and request ip is unlikeness");
                            context.Fail();
                        }
                    }
                }
                else
                {
                    httpContext.Response.Headers.Add("error", "authenticate fail");
                    context.Fail();
                }
            }
            else
            {
                httpContext.Response.Headers.Add("error", "can't find authenticate");
                context.Fail();
            }
        }
    }
}
