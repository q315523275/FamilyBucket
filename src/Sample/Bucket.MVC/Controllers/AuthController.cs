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
            var result = _serviceClient.GetWebApi<string>("Pinzhi.Config.Service",
                "/configs/PinzhiGO/Platform?version=0&sign=d029ccffbe3bf77020fd5da3db3d737cdfd42ae5146ed2df9df29c88f3365b0c",
                 isTrace: true);
            return new OutputLogin { Data = result };
        }

        [HttpGet]
        public string Home()
        {
            return "ok";
        }
    }
}