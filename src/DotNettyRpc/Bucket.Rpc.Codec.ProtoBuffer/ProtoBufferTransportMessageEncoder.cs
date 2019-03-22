using Bucket.Rpc.Codec.ProtoBuffer.Messages;
using Bucket.Rpc.Codec.ProtoBuffer.Utilitys;
using Bucket.Rpc.Messages;
using Bucket.Rpc.Transport.Codec;

namespace Bucket.Rpc.Codec.ProtoBuffer
{
    public class ProtoBufferTransportMessageEncoder : ITransportMessageEncoder
    {
        #region Implementation of ITransportMessageEncoder

        public byte[] Encode(TransportMessage message)
        {
            var transportMessage = new ProtoBufferTransportMessage(message)
            {
                Id = message.Id,
                ContentType = message.ContentType
            };

            return SerializerUtilitys.Serialize(transportMessage);
        }

        #endregion Implementation of ITransportMessageEncoder
    }
}
