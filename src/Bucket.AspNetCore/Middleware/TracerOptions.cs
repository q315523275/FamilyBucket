using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.AspNetCore.Middleware
{
    public class TracerOptions
    {
        /// <summary>
        /// 环境
        /// </summary>
        public string Environment { set; get; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { set; get; }
    }
}
