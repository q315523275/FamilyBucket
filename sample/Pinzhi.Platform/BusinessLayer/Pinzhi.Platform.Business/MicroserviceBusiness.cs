using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bucket.ServiceDiscovery;
using Pinzhi.Platform.DTO;

namespace Pinzhi.Platform.Interface
{
    /// <summary>
    /// 微服务管理业务
    /// </summary>
    public class MicroserviceBusiness : IMicroserviceBusiness
    {
        private readonly IServiceDiscovery _serviceDiscovery;
        public MicroserviceBusiness(IServiceDiscovery serviceDiscovery)
        {
            _serviceDiscovery = serviceDiscovery;
        }

        /// <summary>
        /// 查询服务发现服务列表
        /// </summary>
        /// <returns></returns>
        public async Task<QueryServiceListOutput> QueryServiceList()
        {
            var list = await _serviceDiscovery.FindAllServicesAsync();
            return new QueryServiceListOutput { Data = list };
        }
    }
}
