using Bucket.ApiGateway.Extensions.DotNetty.NettyRequest;
using Bucket.Rpc.Exceptions;
using Bucket.Rpc.ProxyGenerator;
using Ocelot.Logging;
using Ocelot.Middleware;
using Ocelot.Responses;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Bucket.ApiGateway.Extensions.DotNetty
{
    public class DotNettyHttpMiddleware : OcelotMiddleware
    {
        private readonly OcelotRequestDelegate _next;
        private readonly IDotNettyRequestBuilder _dotNettyRequestBuilder;
        private readonly IServiceProxyProvider _serviceProxyProvider;
        public DotNettyHttpMiddleware(
            OcelotRequestDelegate next,
            IDotNettyRequestBuilder dotNettyRequestBuilder,
            IServiceProxyProvider serviceProxyProvider,
            IOcelotLoggerFactory factory) : base(factory.CreateLogger<DotNettyHttpMiddleware>())
        {
            _next = next;
            _dotNettyRequestBuilder = dotNettyRequestBuilder;
            _serviceProxyProvider = serviceProxyProvider;
        }
        public async Task Invoke(DownstreamContext context)
        {
            string resultMessage = string.Empty;
            var httpStatusCode = HttpStatusCode.OK;
            var buildRequest = await _dotNettyRequestBuilder.BuildRequest(context);
            if (buildRequest.IsError)
            {
                resultMessage = "bad request";
                httpStatusCode = HttpStatusCode.BadRequest;
                Logger.LogDebug(resultMessage);
            }
            else
            {
                try
                {
                    var endpoint = new IPEndPoint(IPAddress.Parse(context.DownstreamRequest.Host), context.DownstreamRequest.Port);
                    resultMessage = await _serviceProxyProvider.InvokeAsync<string>(buildRequest.Data, context.DownstreamRequest.AbsolutePath, endpoint);
                }
                catch (RpcException ex)
                {
                    httpStatusCode = HttpStatusCode.InternalServerError;
                    resultMessage = $"rpc exception.";
                    Logger.LogError("apigateway dotnetty client error", ex);
                }
                catch (Exception ex)
                {
                    httpStatusCode = HttpStatusCode.ServiceUnavailable;
                    resultMessage = $"error in request dotnetty service.";
                    Logger.LogError($"{resultMessage}--{context.DownstreamRequest.ToUri()}", ex);
                }
            }

            OkResponse<DotNettyHttpContent> httpResponse = new OkResponse<DotNettyHttpContent>(new DotNettyHttpContent(resultMessage));
            context.HttpContext.Response.ContentType = "application/json";
            context.DownstreamResponse = new DownstreamResponse(httpResponse.Data, httpStatusCode, httpResponse.Data.Headers, "");
        }
    }
}
