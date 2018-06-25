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
    /// 项目控制器
    /// </summary>
    [Produces("application/json")]
    public class ProjectController : Controller
    {
        private readonly IProjectBusiness _projectBusiness;
        public ProjectController(IProjectBusiness projectBusiness)
        {
            _projectBusiness = projectBusiness;
        }
        /// <summary>
        /// 查看项目列表信息
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("/Project/QueryProject")]
        public async Task<QueryProjectOutput> QueryProject()
        {
            return await _projectBusiness.QueryProject();
        }
    }
}