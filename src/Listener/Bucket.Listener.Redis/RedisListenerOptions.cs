namespace Bucket.Listener.Redis
{
    public class RedisListenerOptions
    {
        public string ConnectionString { set; get; }
        public string ListenerKey { set; get; }
    }
}
