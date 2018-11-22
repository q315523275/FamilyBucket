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
    [ApiController]
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
        [Authorize("permission")]
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
        [Authorize("permission")]
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
        [Authorize("permission")]
        public async Task<DeleteServiceOutput> DeleteService([FromBody] DeleteServiceInput input)
        {
            return await _microserviceBusines.DeleteService(input);
        }
        /// <summary>
        /// 查询网关配置
        /// </summary>
        /// <returns></returns>
        [HttpGet("/Microservice/GetApiGatewayConfiguration")]
        [Authorize("permission")]
        public async Task<GetApiGatewayConfigurationOutput> GetApiGatewayConfiguration()
        {
            return await _microserviceBusines.GetApiGatewayConfiguration();
        }
        /// <summary>
        /// 设置网关配置
        /// </summary>
        /// <returns></returns>
        [HttpPost("/Microservice/SetApiGatewayConfiguration")]
        [Authorize("permission")]
        public async Task<BaseOutput> SetApiGatewayConfiguration()
        {
            return await _microserviceBusines.SetApiGatewayConfiguration();
        }
    }
}