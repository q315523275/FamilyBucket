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
        public async Task<QueryServiceListOutput> QueryServiceList(QueryServiceListInput input)
        {
            var result = new object();
            if (input.State == 0)
            {
                result = await _serviceDiscovery.FindAllServicesAsync();
            }
            if (input.State == 1)
            {
                if (string.IsNullOrWhiteSpace(input.Name))
                {
                    result = await _serviceDiscovery.FindServiceInstancesAsync();
                }
                else
                {
                    result = await _serviceDiscovery.FindServiceInstancesAsync(input.Name);
                }
            }
            return new QueryServiceListOutput { Data = result };
        }
        /// <summary>
        /// 服务注册
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SetServiceInfoOutput> SetServiceInfo(SetServiceInfoInput input)
        {
            var result = await _serviceDiscovery.RegisterServiceAsync(input.Name, input.Version, new Uri($"http://{input.HostAndPort.Host}:{input.HostAndPort.Port}"), tags: input.Tags);
            return new SetServiceInfoOutput { };
        }
        /// <summary>
        /// 服务移除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DeleteServiceOutput> DeleteService(DeleteServiceInput input)
        {
            await _serviceDiscovery.DeregisterServiceAsync(input.ServiceId);
            return new DeleteServiceOutput { };
        }

    }
}
