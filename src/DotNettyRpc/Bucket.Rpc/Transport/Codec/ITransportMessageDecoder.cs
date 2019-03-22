using Bucket.Rpc.Messages;

namespace Bucket.Rpc.Transport.Codec
{
    public interface ITransportMessageDecoder
    {
        TransportMessage Decode(byte[] data);
    }
}
