using System.Collections.Concurrent;
namespace Bucket.Config
{
    public class BucketConfig
    {
        public BucketConfig()
        {
            KV = new ConcurrentDictionary<string, string>();
        }

        public string AppName { get; set; }
        public ConcurrentDictionary<string, string> KV { get; set; }
        public long Version { get; set; }
    }
}
