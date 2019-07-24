using Bucket.ApiGateway.Extensions.DotNetty.NettyRequest;
using Bucket.Rpc.Exceptions;
using Bucket.Rpc.ProxyGenerator;
using Newtonsoft.Json;
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
            var httpStatusCode = HttpStatusCode.OK;
            var buildRequest = await _dotNettyRequestBuilder.BuildRequest(context);
            string resultMessage;
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
                    var serviceId = context.DownstreamRequest.AbsolutePath.TrimStart('/');
                    var result = await _serviceProxyProvider.InvokeAsync<object>(buildRequest.Data, serviceId, endpoint);
                    resultMessage = JsonConvert.SerializeObject(result);
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
                    resultMessage = $"error in request netty service.";
                    Logger.LogError($"{resultMessage}--{context.DownstreamRequest.ToUri()}", ex);
                }
            }

            OkResponse<DotNettyHttpContent> httpResponse = new OkResponse<DotNettyHttpContent>(new DotNettyHttpContent(resultMessage));
            //context.HttpContext.Response.ContentType = "application/json";
            context.DownstreamResponse = new DownstreamResponse(httpResponse.Data, httpStatusCode, httpResponse.Data.Headers, string.Empty);
        }
    }
}
