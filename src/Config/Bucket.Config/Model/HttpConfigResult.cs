using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Config
{
    public class HttpConfigResult
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string AppName { get; set; }
        public ConcurrentDictionary<string, string> KV { get; set; }
        public long Version { get; set; }
    }
}
