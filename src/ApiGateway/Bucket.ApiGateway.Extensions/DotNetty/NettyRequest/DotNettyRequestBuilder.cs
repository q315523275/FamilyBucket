using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ocelot.Logging;
using Ocelot.Middleware;
using Ocelot.Request.Mapper;
using Ocelot.Responses;

namespace Bucket.ApiGateway.Extensions.DotNetty.NettyRequest
{
    public class DotNettyRequestBuilder : IDotNettyRequestBuilder
    {
        private readonly IOcelotLogger logger;

        public DotNettyRequestBuilder(IOcelotLoggerFactory factory)
        {
            this.logger = factory.CreateLogger<DotNettyRequestBuilder>();
        }

        public async Task<Response<IDictionary<string, object>>> BuildRequest(DownstreamContext context)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            try
            {
                var httpContext = context.HttpContext;
                if (httpContext.Request.HasFormContentType)
                {
                    var collection = httpContext.Request.Form.ToDictionary(p => p.Key, p => (object)p.Value.ToString());
                    parameters.Add("form", collection);
                }
                else if (httpContext.Request.Method == "GET")
                {
                    parameters = httpContext.Request.Query.ToDictionary(p => p.Key, p => (object)p.Value.ToString());
                }
                else if (httpContext.Request.Method == "POST")
                {
                    var httpRequestMessage = context.DownstreamRequest.ToHttpRequestMessage();
                    var data = await httpRequestMessage.Content.ReadAsStringAsync();
                    parameters = JsonConvert.DeserializeObject<IDictionary<string, object>>(data) ?? new Dictionary<string, object>();
                }
            }
            catch (Exception)
            {
                return SetError("request parameter error");
            }
            context.DownstreamRequest.Scheme = "netty";
            return new OkResponse<IDictionary<string, object>>(parameters);
        }
        ErrorResponse<IDictionary<string, object>> SetError(Exception exception)
        {
            return new ErrorResponse<IDictionary<string, object>>(new UnmappableRequestError(exception));
        }
        ErrorResponse<IDictionary<string, object>> SetError(string message)
        {
            var exception = new Exception(message);
            return SetError(exception);
        }
    }
}
