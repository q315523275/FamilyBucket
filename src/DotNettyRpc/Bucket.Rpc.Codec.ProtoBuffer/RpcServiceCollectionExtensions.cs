namespace Bucket.Rpc.Codec.ProtoBuffer
{
    public static class RpcServiceCollectionExtensions
    {
        /// <summary>
        /// 使用ProtoBuffer编解码器。
        /// </summary>
        /// <param name="builder">Rpc服务构建者。</param>
        /// <returns>Rpc服务构建者。</returns>
        public static IRpcBuilder UseProtoBufferCodec(this IRpcBuilder builder)
        {
            return builder.UseCodec<ProtoBufferTransportMessageCodecFactory>();
        }
    }
}
