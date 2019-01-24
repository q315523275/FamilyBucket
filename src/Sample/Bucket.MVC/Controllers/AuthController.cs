using Microsoft.AspNetCore.Mvc;
using Bucket.MVC.Models.Dto;
using Bucket.WebApi;
using System.Net.Http;
using System.Threading.Tasks;

using Bucket.DbContext;
using Bucket.MVC.Models;
using Pinzhi.Credit.Model;

namespace Bucket.MVC.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthController : ApiControllerBase
    {
        private readonly IDbRepository<WoPayUserInfo> _userRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IDbRepository<WoPayUserInfo> userRepository, IHttpClientFactory httpClientFactory)
        {
            _userRepository = userRepository;
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
            return new OutputLogin {  };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("/User")]
        public object User()
        {
            var c = _userRepository.UseDb("localhost").Count(it => 1 == 1);
            var s = _userRepository.UseDb("111").Count(it => 1 == 1);
            return c + "---" + s;
        }
    }
}