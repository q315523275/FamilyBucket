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
using Rabbit.Zookeeper.Implementation;
using System.Text;
using Bucket.Listener.Abstractions;

namespace Bucket.MVC.Controllers
{
    [Produces("application/json")]
    public class AuthController : ApiControllerBase
    {
        private readonly RedisClient _redisClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPublishCommand _publishCommand;

        public AuthController(RedisClient redisClient, IConfiguration configuration, IHttpClientFactory httpClientFactory, IPublishCommand publishCommand)
        {
            _redisClient = redisClient;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _publishCommand = publishCommand;
        }



        /// <summary>
        /// 111
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("/auth/login")]
        public async Task<OutputLogin> Login([FromBody] InputLogin input)
        {
            var client = _httpClientFactory.CreateClient();
            await client.GetStringAsync("http://www.baidu.com");
            return new OutputLogin {  };
        }
        [HttpGet("/bucket/set")]
        public string User()
        {
            _publishCommand.PublishCommandMessage("Bucket.Sample", new Values.NetworkCommand { NotifyComponent = "Bucket.Config", CommandText = "ConfigRefresh" }).GetAwaiter();
            return "0";
        }
        [HttpGet("/config/{key}")]
        public string config(string key)
        {
            return _configuration.GetValue<string>(key);
        }
    }
}