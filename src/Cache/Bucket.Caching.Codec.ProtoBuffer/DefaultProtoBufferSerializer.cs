using Bucket.Caching.Abstractions;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bucket.Caching.Codec.ProtoBuffer
{
    public class DefaultProtoBufferSerializer : ICachingSerializer
    {
        public T Deserialize<T>(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }

        public object Deserialize(byte[] bytes, Type type)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return Serializer.Deserialize(type, ms);
            }
        }

        public byte[] Serialize<T>(T value)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize<T>(ms, value);
                return ms.ToArray();
            }
        }
    }
}
