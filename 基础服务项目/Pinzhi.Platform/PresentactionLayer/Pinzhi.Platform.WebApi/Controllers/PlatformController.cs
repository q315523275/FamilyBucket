using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pinzhi.Platform.Interface;
using Pinzhi.Platform.DTO;
using Microsoft.AspNetCore.Authorization;

namespace Pinzhi.Platform.WebApi.Controllers
{
    /// <summary>
    /// 平台控制器
    /// </summary>
    [Produces("application/json")]
    public class PlatformController : Controller
    {
        private readonly IPlatformBusiness _platformBusiness;
        public PlatformController(IPlatformBusiness platformBusiness)
        {
            _platformBusiness = platformBusiness;
        }
        /// <summary>
        /// 查询平台列表
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("/Platform/QueryPlatforms")]
        public async Task<QueryPlatformsOutput> QueryPlatforms()
        {
            return await _platformBusiness.QueryPlatforms();
        }
        /// <summary>
        /// 设置平台信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("/Platform/SetPlatform")]
        public async Task<SetPlatformOutput> SetPlatform([FromBody]SetPlatformInput input)
        {
            return await _platformBusiness.SetPlatform(input);
        }
    }
}