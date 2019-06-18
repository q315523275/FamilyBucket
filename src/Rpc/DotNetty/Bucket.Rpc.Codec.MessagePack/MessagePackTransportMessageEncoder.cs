
using Bucket.Rpc.Codec.MessagePack.Messages;
using Bucket.Rpc.Codec.MessagePack.Utilities;
using Bucket.Rpc.Messages;
using Bucket.Rpc.Transport.Codec;
using System.Runtime.CompilerServices;

namespace Bucket.Rpc.Codec.MessagePack
{
    public sealed class MessagePackTransportMessageEncoder : ITransportMessageEncoder
    {
        #region Implementation of ITransportMessageEncoder

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] Encode(TransportMessage message)
        {
            var transportMessage = new MessagePackTransportMessage(message)
            {
                Id = message.Id,
                ContentType = message.ContentType,
            };
            return SerializerUtilitys.Serialize(transportMessage);
        }
        #endregion Implementation of ITransportMessageEncoder
    }
}
