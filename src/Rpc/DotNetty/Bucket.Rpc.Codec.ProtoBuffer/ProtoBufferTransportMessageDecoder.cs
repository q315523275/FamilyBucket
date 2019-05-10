using Bucket.Rpc.Codec.ProtoBuffer.Messages;
using Bucket.Rpc.Codec.ProtoBuffer.Utilitys;
using Bucket.Rpc.Messages;
using Bucket.Rpc.Transport.Codec;

namespace Bucket.Rpc.Codec.ProtoBuffer
{
    public class ProtoBufferTransportMessageDecoder : ITransportMessageDecoder
    {
        #region Implementation of ITransportMessageDecoder

        public TransportMessage Decode(byte[] data)
        {
            var message = SerializerUtilitys.Deserialize<ProtoBufferTransportMessage>(data);

            return message.GetTransportMessage();
        }

        #endregion Implementation of ITransportMessageDecoder
    }
}
