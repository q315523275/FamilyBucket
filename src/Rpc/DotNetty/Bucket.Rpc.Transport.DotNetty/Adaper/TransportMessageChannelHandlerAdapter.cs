using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Bucket.Rpc.Transport.Codec;

namespace Bucket.Rpc.Transport.DotNetty.Adaper
{
    internal class TransportMessageChannelHandlerAdapter : ChannelHandlerAdapter
    {
        private readonly ITransportMessageDecoder _transportMessageDecoder;

        public TransportMessageChannelHandlerAdapter(ITransportMessageDecoder transportMessageDecoder)
        {
            _transportMessageDecoder = transportMessageDecoder;
        }

        #region Overrides of ChannelHandlerAdapter

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = (IByteBuffer)message;
            var data = new byte[buffer.ReadableBytes];
            buffer.ReadBytes(data);
            var transportMessage = _transportMessageDecoder.Decode(data);
            context.FireChannelRead(transportMessage);
        }

        #endregion Overrides of ChannelHandlerAdapter
    }
}
