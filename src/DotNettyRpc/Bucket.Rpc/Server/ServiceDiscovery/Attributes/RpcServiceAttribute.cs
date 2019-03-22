using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Rpc.Server.ServiceDiscovery.Attributes
{
    /// <summary>
    /// 服务标记。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class RpcServiceAttribute : RpcServiceDescriptorAttribute
    {
        /// <summary>
        /// 初始化一个新的Rpc服务标记。
        /// </summary>
        public RpcServiceAttribute()
        {
            IsWaitExecution = true;
        }

        /// <summary>
        /// 是否需要等待服务执行。
        /// </summary>
        public bool IsWaitExecution { get; set; }

        /// <summary>
        /// 路由地址
        /// </summary>
        public string Route { get; set; }

        #region Overrides of RpcServiceDescriptorAttribute

        /// <summary>
        /// 应用标记。
        /// </summary>
        /// <param name="descriptor">服务描述符。</param>
        public override void Apply(ServiceDescriptor descriptor)
        {
            descriptor.WaitExecution(IsWaitExecution);
        }

        #endregion Overrides of RpcServiceDescriptorAttribute
    }
}
