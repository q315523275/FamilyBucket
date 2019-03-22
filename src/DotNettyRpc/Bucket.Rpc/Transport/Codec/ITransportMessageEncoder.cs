using Bucket.Rpc.Messages;

namespace Bucket.Rpc.Transport.Codec
{
    public interface ITransportMessageEncoder
    {
        byte[] Encode(TransportMessage message);
    }
}
