namespace Bucket.SkrTrace.Core
{
    public class SkrTraceOptions
    {
        /// <summary>
        /// 系统应用名称
        /// </summary>
        public string ApplicationCode { set; get; }
        /// <summary>
        /// 过滤正则
        /// </summary>
        public string[] IgnoredRoutesRegexPatterns { set; get; }
        /// <summary>
        /// 追踪HttpContent
        /// </summary>
        public bool TraceHttpBody { set; get; }
        /// <summary>
        /// Maximum storage in transmission interval
        /// </summary>
        public int PendingSegmentLimit { get; set; } = 5000;
        /// <summary>
        /// Flush Interval Millisecond
        /// </summary>
        public int Interval { get; set; } = 3000;
    }
}
