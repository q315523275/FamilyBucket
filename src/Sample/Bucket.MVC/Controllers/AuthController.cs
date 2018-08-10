using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Bucket.MVC.Models.Dto;
using SqlSugar;
using Bucket.WebApi;
using System.Net.Http;
using System.Threading.Tasks;

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
        public AuthController(SqlSugarClient dbContext, ILogger<AuthController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("/authapi/login")]
        public async Task<OutputLogin> Login([FromBody] InputLogin input)
        {
            return new OutputLogin {  };
        }
    }
}