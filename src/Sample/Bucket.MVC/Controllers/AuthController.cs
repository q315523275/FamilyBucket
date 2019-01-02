using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Bucket.MVC.Models.Dto;
using SqlSugar;
using Bucket.WebApi;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Authorization;
using Bucket.Redis;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
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
        private readonly RedisClient _redisClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(ILogger<AuthController> logger, RedisClient redisClient, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _redisClient = redisClient;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }



        /// <summary>
        /// 111
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("/auth/login")]
        public async Task<OutputLogin> Login([FromBody] InputLogin input)
        {
            //Request.EnableRewind();
            //Request.Body.Position = 0;
            //using (var reader = new StreamReader(Request.Body))
            //{
            //    var body = reader.ReadToEnd();
            //    Request.Body.Seek(0, SeekOrigin.Begin);
            //    body = reader.ReadToEnd();
            //}
            var client = _httpClientFactory.CreateClient();
            await client.GetStringAsync("http://www.baidu.com");
            return new OutputLogin {  };
        }
        [HttpGet("/bucket/set")]
        public string User()
        {
            var _redis = _redisClient.GetSubscriber("127.0.0.1:6379,allowadmin=true");
            _redis.Publish("Bucket.Listener.Bucket.Sample", JsonConvert.SerializeObject(new { NotifyComponent = "Bucket.Authorize", CommandText = "AuthorizeReload" }));
            _redis.Publish("Bucket.Listener.Bucket.Sample", JsonConvert.SerializeObject(new { NotifyComponent = "Bucket.Config", CommandText = "ConfigRefresh" }));
            _redis.Publish("Bucket.Listener.Bucket.Sample", JsonConvert.SerializeObject(new { NotifyComponent = "Bucket.ErrorCode", CommandText = "ErrorCodeReload" }));
            return "0";
        }
        [HttpGet("/config/{key}")]
        public string config(string key)
        {
            return _configuration.GetValue<string>(key);
        }
    }
}