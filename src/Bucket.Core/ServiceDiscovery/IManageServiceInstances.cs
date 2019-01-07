using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bucket.Values;
namespace Bucket.ServiceDiscovery
{
    public interface IManageServiceInstances
    {
        /// <summary>
        /// 注册服务实例
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="version">版本号</param>
        /// <param name="uri">服务地址</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="healthCheckUri">健康检查url</param>
        /// <param name="tags">标签</param>
        /// <returns></returns>
        Task<ServiceInformation> RegisterServiceAsync(string serviceName, string version, Uri uri, ServiceType serviceType = ServiceType.HTTP, Uri healthCheckUri = null, IEnumerable<string> tags = null);

        /// <summary>
        /// 注销服务实例
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        Task<bool> DeregisterServiceAsync(string serviceId);
    }
}
