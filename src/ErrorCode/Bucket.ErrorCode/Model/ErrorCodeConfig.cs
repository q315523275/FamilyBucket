using System.Collections.Concurrent;

namespace Bucket.ErrorCode.Model
{
    public class ErrorCodeConfig
    {
        public ConcurrentDictionary<string, string> KV { get; set; }
    }
}
