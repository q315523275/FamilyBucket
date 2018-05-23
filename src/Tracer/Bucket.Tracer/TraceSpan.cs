using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracer
{
    public class TraceSpan
    {
        /// <summary>
        /// TraceId
        /// </summary>
        public string TraceId { set; get; }

        /// <summary>
        /// 编号
        /// </summary>
        public string SpanId { get; set; }

        /// <summary>
        /// 父级编号
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 发起人
        /// </summary>
        public string LaunchId { get; set; }

        /// <summary>
        /// 请求开始时间
        /// </summary>
        public DateTime StartTime { set; get; }

        /// <summary>
        /// 响应时间
        /// </summary>
        public DateTime EndTime { set; get; }

        /// <summary>
        /// TotalMilliseconds
        /// </summary>
        public double TimeLength { set; get; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { set; get; }

        /// <summary>
        /// Api请求地址/方法名
        /// </summary>
        public string OperationName { get; set; }

        /// <summary>
        /// 是否运行成功,true|成功,false|失败
        /// </summary>
        public bool IsSuccess { set; get; } = true;

        /// <summary>
        /// 扩展标签
        /// </summary>
        public TraceTags Tags { get; set; }
    }
}
