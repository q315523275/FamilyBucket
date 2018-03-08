using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Core
{
    public interface IJsonHelper
    {
        string SerializeObject(object value);
        T DeserializeObject<T>(string value);
    }
}
