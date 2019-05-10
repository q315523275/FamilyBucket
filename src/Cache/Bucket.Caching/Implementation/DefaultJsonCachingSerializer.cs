using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Bucket.Caching.Abstractions;
using Newtonsoft.Json;

namespace Bucket.Caching.Implementation
{
    public class DefaultJsonCachingSerializer : ICachingSerializer
    {
        private readonly JsonSerializer jsonSerializer;

        public DefaultJsonCachingSerializer()
        {
            jsonSerializer = new JsonSerializer { };
        }

        public T Deserialize<T>(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                using (var sr = new StreamReader(ms, Encoding.UTF8))
                {
                    using (var jtr = new JsonTextReader(sr))
                    {
                        return jsonSerializer.Deserialize<T>(jtr);
                    }
                }
            }
        }

        public object Deserialize(byte[] bytes, Type type)
        {
            using (var ms = new MemoryStream(bytes))
            {
                using (var sr = new StreamReader(ms, Encoding.UTF8))
                {
                    using (var jtr = new JsonTextReader(sr))
                    {
                        return jsonSerializer.Deserialize(jtr, type);
                    }
                }
            }
        }

        public byte[] Serialize<T>(T value)
        {
            using (var ms = new MemoryStream())
            {
                using (var sr = new StreamWriter(ms, Encoding.UTF8))
                {
                    using (var jtr = new JsonTextWriter(sr))
                    {
                        jsonSerializer.Serialize(jtr, value);
                    }
                    return ms.ToArray();
                }
            }
        }
    }
}
