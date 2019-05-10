namespace Bucket.Caching.StackExchangeRedis.Abstractions
{
    public class StackExchangeRedisOption
    {
        /// <summary>
        /// 提供者名称
        /// </summary>
        public string DbProviderName { set; get; }
        /// <summary>
        /// StackExchangeRedis连接字符串
        /// </summary>
        public string Configuration { set; get; }
    }
}
