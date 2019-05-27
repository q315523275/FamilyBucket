namespace Bucket.Rpc.Codec.MessagePack
{
    public static class RpcServiceCollectionExtensions
    {
        /// <summary>
        /// 使用MessagePack编解码器
        /// </summary>
        /// <param name="builder">Rpc服务构建者。</param>
        /// <returns>Rpc服务构建者。</returns>
        public static IRpcBuilder UseMessagePackCodec(this IRpcBuilder builder)
        {
            return builder.UseCodec<MessagePackTransportMessageCodecFactory>();
        }
    }
}
