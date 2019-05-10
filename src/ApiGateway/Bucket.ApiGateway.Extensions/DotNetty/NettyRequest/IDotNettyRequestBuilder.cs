using Ocelot.Middleware;
using Ocelot.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bucket.ApiGateway.Extensions.DotNetty.NettyRequest
{
    public interface IDotNettyRequestBuilder
    {
        Task<Response<IDictionary<string, object>>> BuildRequest(DownstreamContext context);
    }
}
