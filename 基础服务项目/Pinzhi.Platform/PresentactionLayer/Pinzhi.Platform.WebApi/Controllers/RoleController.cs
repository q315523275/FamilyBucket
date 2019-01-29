using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pinzhi.Platform.Interface;
using Pinzhi.Platform.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Pinzhi.Platform.WebApi.Controllers
{
    /// <summary>
    /// 角色控制器
    /// </summary>
    [Authorize("permission")]
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
        [HttpGet("/Role/QueryAllRoles")]
        public async Task<QueryRolesOutput> QueryAllRoles([FromQuery] QueryRolesInput input)
        {
            return await _roleBusiness.QueryAllRoles(input);
        }
        /// <summary>
        /// 查询可用角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("/Role/QueryRoles")]
        public async Task<QueryRolesOutput> QueryRoles([FromQuery] QueryRolesInput input)
        {
            return await _roleBusiness.QueryRoles(input);
        }
        /// <summary>
        /// 查询角色权限信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("/Role/QueryRoleInfo")]
        public async Task<QueryRoleInfoOutput> QueryRoleInfo([FromQuery] QueryRoleInfoInput input)
        {
            return await _roleBusiness.QueryRoleInfo(input);
        }
        /// <summary>
        /// 设置角色信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("/Role/SetRole")]
        public async Task<SetRoleOutput> SetRole([FromBody] SetRoleInput input)
        {
            return await _roleBusiness.SetRole(input);
        }
    }
}