using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Bucket.Config;
using Bucket.DbContext;
using Bucket.ServiceDiscovery;
using Bucket.Utility.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json.Linq;
using Pinzhi.Platform.Dto;
using Pinzhi.Platform.Dto.Microservice;
using Pinzhi.Platform.Model;

namespace Pinzhi.Platform.Interface
{
    /// <summary>
    /// 微服务管理业务
    /// </summary>
    public class MicroserviceBusiness : IMicroserviceBusiness
    {
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly IConfig _config;
        private readonly IDbRepository<ApiGatewayConfigurationInfo> _apigatewayRepository;

        public MicroserviceBusiness(IServiceDiscovery serviceDiscovery, IConfig config, IDbRepository<ApiGatewayConfigurationInfo> apigatewayRepository)
        {
            _serviceDiscovery = serviceDiscovery;
            _config = config;
            _apigatewayRepository = apigatewayRepository;
        }




        /// <summary>
        /// 查询服务发现服务列表
        /// </summary>
        /// <returns></returns>
        public async Task<QueryServiceListOutput> QueryServiceList(QueryServiceListInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                return new QueryServiceListOutput { Data = await _serviceDiscovery.FindServiceInstancesAsync() };
            }
            else
            {
                return new QueryServiceListOutput { Data = await _serviceDiscovery.FindServiceInstancesAsync(input.Name) };
            }
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
            try
            {
                _apigatewayRepository.DbContext.Ado.BeginTran();

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

                var config = await _apigatewayRepository.GetFirstAsync(it => it.ConfigurationKey == key);
                if (config == null)
                    await _apigatewayRepository.InsertAsync(new ApiGatewayConfigurationInfo { ConfigurationKey = key, Configuration = bodyStr });
                else
                    await _apigatewayRepository.UpdateAsync(new ApiGatewayConfigurationInfo { Id = config.Id, ConfigurationKey = config.ConfigurationKey, Configuration = bodyStr });

                await _serviceDiscovery.KeyValuePutAsync(key, bodyStr);

                _apigatewayRepository.DbContext.Ado.CommitTran();
            }
            catch(Exception ex)
            {
                _apigatewayRepository.DbContext.Ado.RollbackTran();
                throw new Exception("网关配置更新失败", ex);
            }
            return new BaseOutput { };
        }
    }
}
