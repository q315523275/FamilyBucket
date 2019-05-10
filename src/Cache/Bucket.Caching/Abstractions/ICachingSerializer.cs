using System;

namespace Bucket.Caching.Abstractions
{
    public interface ICachingSerializer
    {
        byte[] Serialize<T>(T value);
        T Deserialize<T>(byte[] bytes);
        object Deserialize(byte[] bytes, Type type);
    }
}
