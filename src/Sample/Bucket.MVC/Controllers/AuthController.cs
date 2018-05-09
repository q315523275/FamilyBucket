using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Bucket.MVC.Models.Dto;
using SqlSugar;
using Bucket.WebApi;
using Bucket.ServiceClient;

namespace Bucket.MVC.Controllers
{
    [Produces("application/json")]
    public class AuthController : ApiControllerBase
    {
        /// <summary>
        /// 自定义策略参数
        /// </summary>
        private readonly SqlSugarClient _dbContext;
        private readonly ILogger<AuthController> _logger;
        private readonly IServiceClient _serviceClient;
        public AuthController(SqlSugarClient dbContext, ILogger<AuthController> logger, IServiceClient serviceClient)
        {
            _dbContext = dbContext;
            _logger = logger;
            _serviceClient = serviceClient;
        }
       
       
        [HttpPost("/authapi/login")]
        public OutputLogin Login([FromBody] InputLogin input)
        {
            var result = _serviceClient.PostWebApi<string>("http://api.51pinzhi.cn/order/api/Query/QueryPrePayIP8",new { StartTime = "2017-05-09 14:35:14", EndTime = "2018-05-09 14:35:14" }, isTrace: true);
            var result2 = _serviceClient.PostWebApi<string>("http://api.51pinzhi.cn/order/api/Query/QueryPayedOrderCount", new { OrderType = 0, OrderChannel = 0 }, isTrace: true);
            return new OutputLogin { Data = result + result2 };
        }

        [HttpGet]
        public string Home()
        {
            return "ok";
        }
    }
}