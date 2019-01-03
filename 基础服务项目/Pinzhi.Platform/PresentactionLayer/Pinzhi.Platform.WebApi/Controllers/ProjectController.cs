using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pinzhi.Platform.Interface;
using Pinzhi.Platform.Dto.Project;
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
        [Authorize("permission")]
        [HttpGet("/Project/QueryProject")]
        public async Task<QueryProjectOutput> QueryProject()
        {
            return await _projectBusiness.QueryProject();
        }
        /// <summary>
        /// 设置项目信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize("permission")]
        [HttpPost("/Project/SetProject")]
        public async Task<SetProjectOutput> SetProject([FromBody] SetProjectInput input)
        {
            return await _projectBusiness.SetProject(input);
        }
        /// <summary>
        /// 推送项目信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize("permission")]
        [HttpPost("/Project/PublishCommand")]
        public async Task<PublishCommandOutput> PublishCommand([FromBody] PublishCommandInput input)
        {
            return await _projectBusiness.PublishCommand(input);
        }
    }
}