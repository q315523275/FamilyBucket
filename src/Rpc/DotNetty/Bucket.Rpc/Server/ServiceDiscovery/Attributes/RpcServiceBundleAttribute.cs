using System;

namespace Bucket.Rpc.Server.ServiceDiscovery.Attributes
{
    /// <summary>
    /// 服务集标记。
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class RpcServiceBundleAttribute : Attribute
    {
    }
}
