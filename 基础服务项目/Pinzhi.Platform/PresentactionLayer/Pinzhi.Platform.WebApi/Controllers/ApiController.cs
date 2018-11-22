using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pinzhi.Platform.Interface;
using Pinzhi.Platform.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Pinzhi.Platform.WebApi.Controllers
{
    /// <summary>
    /// Api资源控制器
    /// </summary>
    [Produces("application/json")]
    public class ApiController : Controller
    {
        private readonly IApiBusiness _apiBusiness;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="apiBusiness"></param>
        public ApiController(IApiBusiness apiBusiness)
        {
            _apiBusiness = apiBusiness;
        }
        /// <summary>
        /// 查询Api资源
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize("permission")]
        [HttpGet("/Api/QueryApiList")]
        public async Task<QueryApisOutput> QueryApiList([FromQuery] QueryApisInput input)
        {
            return await _apiBusiness.QueryApis(input);
        }
        /// <summary>
        /// 设置Api资源
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize("permission")]
        [HttpPost("/Api/SetApi")]
        public async Task<SetApiOutput> SetApi([FromBody] SetApiInput input)
        {
            return await _apiBusiness.SetApi(input);
        }
    }
}