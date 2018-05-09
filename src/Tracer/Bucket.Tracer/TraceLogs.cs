using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracer
{
    public class TraceLogs
    {
        /// <summary>
        /// TraceId
        /// </summary>
        public string TraceId { set; get; }

        /// <summary>
        /// 当前队列顺序号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 父级队列序号
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 顺序Id
        /// </summary>
        public int SortId { get; set; }

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
        /// 是否运行成功,true|成功,false|失败
        /// </summary>
        public bool IsSuccess { set; get; }

        /// <summary>
        /// 是否运行异常,true|异常,false|未异常
        /// </summary>
        public bool IsException { set; get; }

        /// <summary>
        /// 返回状态值
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        /// 环境
        /// </summary>
        public string Environment { set; get; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { set; get; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ModName { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string ApiUri { get; set; }

        /// <summary>
        /// ContextType
        /// </summary>
        public string ContextType { set; get; }

        /// <summary>
        /// 请求内容
        /// </summary>
        public string Request { set; get; }

        /// <summary>
        /// 请求返回
        /// </summary>
        public string Response { set; get; }
    }
}
