using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.OpenTracing
{
    public interface ICarrier : IEnumerable<KeyValuePair<string, string>>
    {
        string this[string key] { get; set; }
    }
}