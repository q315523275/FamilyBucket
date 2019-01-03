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
        /// 更新
        /// </summary>
        Refresh,
        /// <summary>
        /// 重载
        /// </summary>
        Reload,
    }
}