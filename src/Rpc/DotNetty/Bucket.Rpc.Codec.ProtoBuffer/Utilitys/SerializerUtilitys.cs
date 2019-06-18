using ProtoBuf;
using System;
using System.IO;

namespace Bucket.Rpc.Codec.ProtoBuffer.Utilitys
{
    public static class SerializerUtilitys
    {
        public static byte[] Serialize(object instance)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, instance);
                return stream.ToArray();
            }
        }

        public static object Deserialize(byte[] data, Type type)
        {
            if (data == null)
                return null;
            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize(type, stream);
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }
    }
}
