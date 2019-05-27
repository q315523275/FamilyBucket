using Ocelot.Authentication.Middleware;
using Ocelot.Authorisation.Middleware;
using Ocelot.Cache.Middleware;
using Ocelot.DownstreamUrlCreator.Middleware;
using Ocelot.Headers.Middleware;
using Ocelot.LoadBalancer.Middleware;
using Ocelot.Middleware;
using Ocelot.Middleware.Pipeline;
using Ocelot.RateLimit.Middleware;
using Ocelot.Request.Middleware;
using Ocelot.RequestId.Middleware;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.ApiGateway.Extensions.DotNetty
{
    public class DotNettyOcelotPipeline
    {
        public Func<IOcelotPipelineBuilder, Func<DownstreamContext, bool>> DotNettyPipeline = (builder) =>
        {
            // 因为DotNetty配置组件未完成,暂时使用网关控制
            builder.UseHttpHeadersTransformationMiddleware();
            builder.UseDownstreamRequestInitialiser();
            builder.UseRateLimiting(); // 限速
            builder.UseRequestIdMiddleware(); 
            builder.UseAuthenticationMiddleware(); // 认证
            builder.UseAuthorisationMiddleware(); // 授权
            builder.UseLoadBalancingMiddleware(); // 负载
            builder.UseDownstreamUrlCreatorMiddleware(); // HTTP URL组合
            builder.UseOutputCacheMiddleware();
            // 添加DotNetty请求中间件
            builder.UseDotNettyHttpMiddleware();
            // 路由配置页面设置为DotNetty请求
            return (context) => context.DownstreamReRoute?.DownstreamScheme?.ToLower() == "dotnetty";
        };
    }
}
