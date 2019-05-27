using Bucket.Caching.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bucket.Caching.Codec.MessagePack
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 使用MessagePack编解码器
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ICachingBuilder UseMessagePackCodec(this ICachingBuilder builder)
        {
            builder.Services.AddSingleton<ICachingSerializer, DefaultMessagePackSerializer>();
            return builder;
        }
    }
}
