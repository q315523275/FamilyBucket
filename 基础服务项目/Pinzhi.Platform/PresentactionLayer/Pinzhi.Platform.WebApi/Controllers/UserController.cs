using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pinzhi.Platform.Interface;
using Pinzhi.Platform.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Pinzhi.Platform.WebApi.Controllers
{
    [Produces("application/json")]
    public class UserController : Controller
    {
        private readonly IUserBusiness _userBusiness;
        public UserController(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }
        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize("permission")]
        [HttpGet("/User/QueryUsers")]
        public async Task<QueryUsersOutput> QueryUsers([FromQuery] QueryUsersInput input)
        {
            return await _userBusiness.QueryUsers(input);
        }

        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize("permission")]
        [HttpPost("/User/SetUser")]
        public async Task<SetUserOutput> SetUser([FromBody] SetUserInput input)
        {
            return await _userBusiness.SetUser(input);
        }
    }
}