using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Bucket.MVC.Models.Dto;
using SqlSugar;
using Bucket.WebApi;
using Bucket.ServiceClient;
using System.Net.Http;

namespace Bucket.MVC.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    public class AuthController : ApiControllerBase
    {
        /// <summary>
        /// 
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("/authapi/login")]
        public OutputLogin Login([FromBody] InputLogin input,[FromServices] HttpClient httpClient)
        {
            return new OutputLogin { Data = httpClient.PostAsync("http://api.51pinzhi.cn/order/api/Query/QueryPrePayIP8",new StringContent("{}",System.Text.Encoding.UTF8, "application/json")).Result.Content.ReadAsStringAsync().Result };
        }
    }
}