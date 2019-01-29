using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pinzhi.Platform.Dto;
using Pinzhi.Platform.Dto.Microservice;
using Pinzhi.Platform.Interface;

namespace Pinzhi.Platform.WebApi.Controllers
{
    /// <summary>
    /// 微服务管理
    /// </summary>
    [Authorize("permission")]
    public class MicroserviceController : ControllerBase
    {
        private readonly IMicroserviceBusiness _microserviceBusines;
        public MicroserviceController(IMicroserviceBusiness microserviceBusiness)
        {
            _microserviceBusines = microserviceBusiness;
        }
        /// <summary>
        /// 查询服务发现全部服务
        /// </summary>
        /// <returns></returns>
        [HttpGet("/Microservice/QueryServiceList")]
        public async Task<QueryServiceListOutput> QueryServiceList([FromQuery] QueryServiceListInput input)
        {
            return await _microserviceBusines.QueryServiceList(input);
        }
        /// <summary>
        /// 服务注册
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("/Microservice/SetServiceInfo")]
        public async Task<SetServiceInfoOutput> SetServiceInfo([FromBody] SetServiceInfoInput input)
        {
            return await _microserviceBusines.SetServiceInfo(input);
        }
        /// <summary>
        /// 服务移除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("/Microservice/DeleteService")]
        public async Task<DeleteServiceOutput> DeleteService([FromBody] DeleteServiceInput input)
        {
            return await _microserviceBusines.DeleteService(input);
        }
        /// <summary>
        /// 查询网关配置列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("/Microservice/QueryApiGatewayConfiguration")]
        public async Task<QueryApiGatewayConfigurationOutput> QueryApiGatewayConfiguration([FromQuery] QueryApiGatewayConfigurationInput input)
        {
            return await _microserviceBusines.QueryApiGatewayConfiguration(input);
        }
        /// <summary>
        /// 设置网关配置
        /// </summary>
        /// <returns></returns>
        [HttpPost("/Microservice/SetApiGatewayConfiguration")]
        public async Task<BaseOutput> SetApiGatewayConfiguration([FromBody] SetApiGatewayConfigurationInput input)
        {
            return await _microserviceBusines.SetApiGatewayConfiguration(input);
        }
        /// <summary>
        /// 查询网关路由列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("/Microservice/QueryApiGatewayReRouteList")]
        public async Task<QueryApiGatewayReRouteListOutput> QueryApiGatewayReRouteList([FromQuery] QueryApiGatewayReRouteListInput input)
        {
            return await _microserviceBusines.QueryApiGatewayReRouteList(input);
        }
        /// <summary>
        /// 设置网关路由
        /// </summary>
        /// <returns></returns>
        [HttpPost("/Microservice/SetApiGatewayReRoute")]
        public async Task<BaseOutput> SetApiGatewayReRoute([FromBody] SetApiGatewayReRouteInput input)
        {
            return await _microserviceBusines.SetApiGatewayReRoute(input);
        }
        /// <summary>
        /// 同步网关配置到Consul
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("/Microservice/SyncApiGatewayConfigurationToConsul")]
        public async Task<BaseOutput> SyncApiGatewayConfigurationToConsul([FromQuery] SyncApiGatewayConfigurationInput input)
        {
            return await _microserviceBusines.SyncApiGatewayConfigurationToConsul(input);
        }
        /// <summary>
        /// 同步网关配置到Redis
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("/Microservice/SyncApiGatewayConfigurationToRedis")]
        public async Task<BaseOutput> SyncApiGatewayConfigurationToRedis([FromQuery] SyncApiGatewayConfigurationInput input)
        {
            return await _microserviceBusines.SyncApiGatewayConfigurationToRedis(input);
        }
    }
}