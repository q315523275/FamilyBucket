using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bucket.ApiGateway.Extensions.DotNetty.NettyRequest
{
    public class DotNettyHttpContent : HttpContent
    {
        private string result;

        public DotNettyHttpContent(string result)
        {
            this.result = result;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            var writer = new StreamWriter(stream);
            await writer.WriteAsync(result);
            await writer.FlushAsync();
        }

        protected override bool TryComputeLength(out long length)
        {
            length = result.Length;
            return true;
        }
    }
}
