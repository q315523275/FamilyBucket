namespace Bucket.Values
{
    public class NetworkCommand
    {
        public string NotifyComponent { set; get; }
        public string CommandText { set; get; }
    }
    public enum NetworkCommandType
    {
        /// <summary>
        /// 配置刷新
        /// </summary>
        ConfigRefresh,
        /// <summary>
        /// 配置重新加载
        /// </summary>
        ConfigReload,
        /// <summary>
        /// 权限重载
        /// </summary>
        AuthorizeReload,
        /// <summary>
        /// 错误重载
        /// </summary>
        ErrorCodeReload,
        /// <summary>
        /// Jwt黑名单重载
        /// </summary>
        BlackJwtReload,
    }
}