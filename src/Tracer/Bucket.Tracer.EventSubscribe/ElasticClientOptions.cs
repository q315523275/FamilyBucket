using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracer.EventSubscribe
{
    public class ElasticClientOptions
    {
        /// <summary>
        /// 服务地址
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// 默认索引
        /// </summary>
        public string DefaultIndex { get; set; }
    }
}
