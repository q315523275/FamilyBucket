namespace Bucket.Tracing
{
    public class TracingOptions
    {
        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { set; get; }
        public string ServiceIdentity { set; get; }
        public string[] IgnoredRoutesRegexPatterns { set; get; }
        /// <summary>
        /// 追踪HttpContent
        /// </summary>
        public bool TraceHttpContent { set; get; }
    }
}
