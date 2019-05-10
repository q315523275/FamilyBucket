using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bucket.Rpc.Server
{
    /// <summary>
    /// 服务条目。
    /// </summary>
    public class ServiceEntry
    {
        /// <summary>
        /// 执行委托。
        /// </summary>
        public Func<IDictionary<string, object>, Task<object>> Func { get; set; }

        /// <summary>
        /// 服务描述符。
        /// </summary>
        public ServiceDescriptor Descriptor { get; set; }
    }
}
