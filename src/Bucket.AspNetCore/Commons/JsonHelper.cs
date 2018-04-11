using Bucket.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Bucket.AspNetCore.Commons
{
    public class JsonHelper : IJsonHelper
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
