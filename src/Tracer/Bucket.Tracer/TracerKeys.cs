using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracer
{
    public class TracerKeys
    {
        public const string TraceStoreCacheKey = "BucketTrace";

        public const string TraceLaunchId = "Uid";

        public const string TraceModName = "ModName";

        public const string TraceParentSeq = "ParentSeq";

        public const string TraceSeq = "Seq";

        public const string TraceSerial = "TraceSerial";

        public const string TraceId = "TraceId";

        /// <summary>
        /// 环境
        /// </summary>
        public static string Environment { set; get; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public static string SystemName { set; get; }
    }
}
