using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ConfigService.Interface;
using ConfigService.DTO;
using Bucket.WebApi;
namespace ConfigService.WebApi.Controllers
{
    /// <summary>
    /// 配置信息控制器
    /// </summary>
    [Produces("application/json")]
    public class ConfigController : ApiControllerBase
    {
        private readonly IConfigBusniess _configBusniess;
        public ConfigController(IConfigBusniess configBusniess)
        {
            _configBusniess = configBusniess;
        }
        /// <summary>
        /// 查询项目配置信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="namespaceName"></param>
        /// <param name="version"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpGet("/configs/{appId}/{namespaceName}")]
        public async Task<QueryConfigOutput> QueryConfig(string appId, string namespaceName, long version, string env, string sign)
        {
            return await _configBusniess.QueryConfig(new QueryConfigInput { AppId = appId, NamespaceName = namespaceName, Sign = sign, Version = version, Env = env });
        }
    }
}