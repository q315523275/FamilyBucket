using System.Collections.Concurrent;

namespace Bucket.Config
{
    public class ApiResult
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string AppName { get; set; }
        public ConcurrentDictionary<string, string> KV { get; set; }
        public long Version { get; set; }
    }
}
