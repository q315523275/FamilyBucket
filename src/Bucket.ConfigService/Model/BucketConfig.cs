using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Bucket.ConfigCenter
{
    public class BucketConfig
    {
        public ConcurrentDictionary<string, string> KV { get; set; }
        public long Version { get; set; }
    }
}
