using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracer
{
    public class TraceOptions
    {
        /// <summary>
        /// 环境
        /// </summary>
        public string Environment { set; get; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { set; get; }

        /// <summary>
        /// 忽略路径
        /// </summary>
        public string[] IgnoredRoutesRegexPatterns { set; get; }

    }
}
