using Grpc.Core;
using Ocelot.Logging;
using Ocelot.Middleware;
using System.Threading.Tasks;

namespace Bucket.ApiGateway.Extensions.Grpc
{
    public class MagicOnionHttpGatewayMiddleware
    {
        private readonly OcelotRequestDelegate _next;
        readonly Channel channel;

        public MagicOnionHttpGatewayMiddleware(OcelotRequestDelegate next, Channel channel, IOcelotLoggerFactory loggerFactory)
        {
            _next = next;
            this.channel = channel;
        }
        public async Task Invoke(DownstreamContext context)
        {
            var downstreamScheme = context.DownstreamReRoute.DownstreamScheme;
            if (downstreamScheme.ToLower() != "grpc")
            {
                await _next.Invoke(context);
                return;
            }
        }
    }
}
