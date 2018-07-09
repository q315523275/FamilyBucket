using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pinzhi.Platform.DTO;
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
        [Authorize]
        public async Task<QueryServiceListOutput> QueryServiceList([FromQuery]QueryServiceListInput input)
        {
            return await _microserviceBusines.QueryServiceList(input);
        }
        /// <summary>
        /// 服务注册
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("/Microservice/SetServiceInfo")]
        [Authorize]
        public async Task<SetServiceInfoOutput> SetServiceInfo([FromBody]SetServiceInfoInput input)
        {
            return await _microserviceBusines.SetServiceInfo(input);
        }
        /// <summary>
        /// 服务移除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("/Microservice/DeleteService")]
        [Authorize]
        public async Task<DeleteServiceOutput> DeleteService([FromBody]DeleteServiceInput input)
        {
            return await _microserviceBusines.DeleteService(input);
        }
    }
}