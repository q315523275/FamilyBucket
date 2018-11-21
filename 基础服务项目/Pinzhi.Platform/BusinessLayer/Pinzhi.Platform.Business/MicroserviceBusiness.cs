using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Bucket.Config;
using Bucket.ServiceDiscovery;
using Bucket.Utility.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json.Linq;
using Pinzhi.Platform.DTO;
using Pinzhi.Platform.DTO.Microservice;

namespace Pinzhi.Platform.Interface
{
    /// <summary>
    /// 微服务管理业务
    /// </summary>
    public class MicroserviceBusiness : IMicroserviceBusiness
    {
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly IConfig _config;
        public MicroserviceBusiness(IServiceDiscovery serviceDiscovery, IConfig config)
        {
            _serviceDiscovery = serviceDiscovery;
            _config = config;
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
            var result = await _serviceDiscovery.RegisterServiceAsync(serviceName: input.Name, 
                version: input.Version, 
                uri: new Uri($"http://{input.HostAndPort.Host}:{input.HostAndPort.Port}"), 
                healthCheckUri: new Uri(input.HealthCheckUri), 
                tags: input.Tags);
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

        /// <summary>
        /// 查询ApiGatewayConfiguration
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<GetApiGatewayConfigurationOutput> GetApiGatewayConfiguration()
        {
            var key = _config.StringGet("ApiGatewayConfigurationKey");
            var value = await _serviceDiscovery.KeyValueGetAsync(key);
            return new GetApiGatewayConfigurationOutput { Data = JObject.Parse(value) };
        }
        /// <summary>
        /// 设置ApiGatewayConfiguration
        /// </summary>
        /// <returns></returns>
        public async Task<BaseOutput> SetApiGatewayConfiguration()
        {
            #region body
            var req = Web.HttpContext.Request;
            req.EnableRewind();
            var originBody = req.Body;
            req.Body.Position = 0;
            var bodyStr = await new StreamReader(req.Body).ReadToEndAsync();
            req.Body.Position = 0;
            req.Body = originBody;
            #endregion

            var key = _config.StringGet("ApiGatewayConfigurationKey");
            await _serviceDiscovery.KeyValuePutAsync(key, bodyStr);
            return new BaseOutput { };
        }
    }
}
