using Bucket.Core;
using Newtonsoft.Json;
using System.IO;

namespace Bucket.AspNetCore.Serialize
{
    public class DefaultJsonHelper : IJsonHelper
    {
        public T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
