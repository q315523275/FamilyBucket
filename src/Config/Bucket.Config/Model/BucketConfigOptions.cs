namespace Bucket.Config
{
    public class BucketConfigOptions
    {
        public string AppId { set; get; }
        public string AppSercet { set; get; }
        public string NamespaceName { set; get; }
        public string Env { set; get; } = "pro";
        public string ServerUrl { set; get; }
        /// <summary>
        /// 定时器周期，单位（秒）
        /// </summary>
        public int RefreshInteval { set; get; }
        public bool RedisListener { set; get; }
        public string RedisConnectionString { set; get; }

        public bool UseServiceDiscovery { set; get; }
        public string ServiceName { set; get; }
    }
}
