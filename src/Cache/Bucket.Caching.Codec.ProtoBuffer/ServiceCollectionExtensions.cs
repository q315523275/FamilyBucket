using Bucket.Caching.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Caching.Codec.ProtoBuffer
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 使用ProtoBuffer编解码器
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ICachingBuilder UseProtoBufferCodec(this ICachingBuilder builder)
        {
            builder.Services.AddSingleton<ICachingSerializer, DefaultProtoBufferSerializer>();
            return builder;
        }
    }
}
