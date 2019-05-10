using System.Collections.Generic;

namespace Bucket.SkyApm
{
    public class SkyApmConfig
    {
        public string ServiceName { set; get; }
        public List<string> HeaderVersions { set; get; } = new List<string>();
        public SkyApmSamplingOption Sampling { set; get; } = new SkyApmSamplingOption();
        public SkyApmTransportOption Transport { set; get; } = new SkyApmTransportOption();
    }
    public class SkyApmSamplingOption
    {
        /// <summary>
        /// 3秒内最大采集数,小于0不限制
        /// </summary>
        public int SamplePer3Secs { set; get; } = -1;
        /// <summary>
        /// 随机采集百分比,小于或等于0不限制
        /// </summary>
        public double Percentage { set; get; } = -1.0;
    }
    public class SkyApmTransportOption
    {
        /// <summary>
        /// 传输轮询时间,单位秒,默认3000
        /// </summary>
        public int Interval { set; get; } = 3000;
        /// <summary>
        /// 本地队列最大暂存值,默认30000
        /// </summary>
        public int QueueSize { set; get; } = 30000;
        /// <summary>
        /// 传输最大数量,默认3000
        /// </summary>
        public int BatchSize { set; get; } = 3000;
    }

    public static class HeaderVersions
    {
        public static string SW3 { get; } = "sw3";

        public static string SW6 { get; } = "sw6";
        public static string Bucket { get; } = "skyapm";
    }
}
