using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Pinzhi.Platform.DTO;
using Pinzhi.Platform.Interface;
using Microsoft.AspNetCore.Authorization;
using Bucket.WebApi;
namespace Pinzhi.Platform.WebApi.Controllers
{
    /// <summary>
    /// 菜单控制器
    /// </summary>
    [Produces("application/json")]
    public class MenuController : ApiControllerBase
    {
        private readonly IMenuBusiness _menuBusiness;
        public MenuController(IMenuBusiness menuBusiness)
        {
            _menuBusiness = menuBusiness;
        }
        /// <summary>
        /// 查询平台菜单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("/Menu/QueryAllMenus")]
        public async Task<QueryAllMenusOutput> QueryAllMenus(QueryAllMenusInput input)
        {
            return await _menuBusiness.QueryAllMenus(input);
        }
        /// <summary>
        /// 设置平台菜单信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("/Menu/SetPlatform")]
        public async Task<SetMenuOutput> SetPlatform([FromBody]SetMenuInput input)
        {
            return await _menuBusiness.SetPlatform(input);
        }
        /// <summary>
        /// 查询用户菜单
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("/Menu/QueryUserMenus")]
        public async Task<QueryUserMenusOutput> QueryUserMenus()
        {
            return await _menuBusiness.QueryUserMenus();
        }
    }
}