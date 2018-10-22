using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Bucket.MVC.Models.Dto;
using SqlSugar;
using Bucket.WebApi;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http.Internal;

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
            Request.EnableRewind();
            Request.Body.Position = 0;
            using (var reader = new StreamReader(Request.Body))
            {
                var body = reader.ReadToEnd();
                Request.Body.Seek(0, SeekOrigin.Begin);
                body = reader.ReadToEnd();
            }
            return new OutputLogin {  };
        }
    }
}