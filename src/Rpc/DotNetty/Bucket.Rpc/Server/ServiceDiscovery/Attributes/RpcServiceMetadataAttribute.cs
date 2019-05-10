using System;

namespace Bucket.Rpc.Server.ServiceDiscovery.Attributes
{
    /// <summary>
    /// Rpc服务元数据标记。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class RpcServiceMetadataAttribute : RpcServiceDescriptorAttribute
    {
        /// <summary>
        /// 初始化一个新的Rpc服务元数据标记。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="data">数据。</param>
        public RpcServiceMetadataAttribute(string name, object data)
        {
            Name = name;
            Data = data;
        }

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 数据。
        /// </summary>
        public object Data { get; }

        #region Overrides of RpcServiceDescriptorAttribute

        /// <summary>
        /// 应用标记。
        /// </summary>
        /// <param name="descriptor">服务描述符。</param>
        public override void Apply(ServiceDescriptor descriptor)
        {
            descriptor.Metadatas[Name] = Data;
        }

        #endregion Overrides of RpcServiceDescriptorAttribute
    }
}
