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
    /// 角色控制器
    /// </summary>
    [Produces("application/json")]
    public class RoleController : Controller
    {
        private readonly IRoleBusiness _roleBusiness;
        public RoleController(IRoleBusiness roleBusiness)
        {
            _roleBusiness = roleBusiness;
        }
        /// <summary>
        /// 查询所有角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize("permission")]
        [HttpGet("/Role/QueryAllRoles")]
        public async Task<QueryRolesOutput> QueryAllRoles(QueryRolesInput input)
        {
            return await _roleBusiness.QueryAllRoles(input);
        }
        /// <summary>
        /// 查询可用角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize("permission")]
        [HttpGet("/Role/QueryRoles")]
        public async Task<QueryRolesOutput> QueryRoles(QueryRolesInput input)
        {
            return await _roleBusiness.QueryRoles(input);
        }
        /// <summary>
        /// 查询角色权限信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize("permission")]
        [HttpGet("/Role/QueryRoleInfo")]
        public async Task<QueryRoleInfoOutput> QueryRoleInfo(QueryRoleInfoInput input)
        {
            return await _roleBusiness.QueryRoleInfo(input);
        }
        /// <summary>
        /// 设置角色信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize("permission")]
        [HttpPost("/Role/SetRole")]
        public async Task<SetRoleOutput> SetRole([FromBody]SetRoleInput input)
        {
            return await _roleBusiness.SetRole(input);
        }
    }
}