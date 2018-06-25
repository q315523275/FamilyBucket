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
        public async Task<QueryServiceListOutput> QueryServiceList()
        {
            return await _microserviceBusines.QueryServiceList();
        }
    }
}