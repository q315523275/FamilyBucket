
using Bucket.Rpc.Codec.MessagePack.Messages;
using Bucket.Rpc.Codec.MessagePack.Utilities;
using Bucket.Rpc.Messages;
using Bucket.Rpc.Transport.Codec;
using System.Runtime.CompilerServices;

namespace Bucket.Rpc.Codec.MessagePack
{
    public sealed class MessagePackTransportMessageDecoder : ITransportMessageDecoder
    {
        #region Implementation of ITransportMessageDecoder

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TransportMessage Decode(byte[] data)
        {
            var message = SerializerUtilitys.Deserialize<MessagePackTransportMessage>(data);
            return message.GetTransportMessage();
        }

        #endregion Implementation of ITransportMessageDecoder
    }
}
