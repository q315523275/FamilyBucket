using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using Bucket.MVC.Models.Dto;
using SqlSugar;
using Bucket.WebApi;
using Bucket.Buried;
using Bucket.Exceptions;
using System;

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
        public AuthController(SqlSugarClient dbContext, ILogger<AuthController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
       
       
        [HttpPost("/authapi/login")]
        public OutputLogin Login([FromBody] InputLogin input)
        {
            throw new BucketException("GO_0004007", "1");
            return new OutputLogin { Data = input };
        }

        [HttpGet]
        public string Home()
        {
            return "ok";
        }
    }
}