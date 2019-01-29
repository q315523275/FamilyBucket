using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pinzhi.Platform.Dto;
using Pinzhi.Platform.Dto.Config;
using Pinzhi.Platform.Interface;

namespace Pinzhi.Platform.WebApi.Controllers
{
    /// <summary>
    /// 配置管理控制器
    /// </summary>
    [Authorize("permission")]
    public class ConfigController : Controller
    {
        private readonly IConfigBusiness _configBusniess;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configBusniess"></param>
        public ConfigController(IConfigBusiness configBusniess)
        {
            _configBusniess = configBusniess;
        }
        /// <summary>
        /// 查看所有项目组
        /// </summary>
        /// <returns></returns>
        [HttpGet("/Config/QueryAppList")]
        public async Task<QueryAppListOutput> QueryAppList()
        {
            return await _configBusniess.QueryAppList();
        }
        /// <summary>
        /// 设置项目组信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("/Config/SetAppInfo")]
        public async Task<SetAppInfoOutput> SetAppInfo([FromBody] SetAppInfoInput input)
        {
            return await _configBusniess.SetAppInfo(input);
        }
        /// <summary>
        /// 查询配置项目
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("/Config/QueryAppProjectList")]
        public async Task<QueryAppProjectListOutput> QueryAppProjectList([FromQuery] QueryAppProjectListInput input)
        {
            return await _configBusniess.QueryAppProjectList(input);
        }
        /// <summary>
        /// 设置配置项目
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("/Config/SetAppProjectInfo")]
        public async Task<SetAppProjectInfoOutput> SetAppProjectInfo([FromBody] SetAppProjectInfoInput input)
        {
            return await _configBusniess.SetAppProjectInfo(input);
        }
        /// <summary>
        /// 查询配置信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("/Config/QueryAppConfigList")]
        public async Task<QueryAppConfigListOutput> QueryAppConfigList([FromQuery] QueryAppConfigListInput input)
        {
            return await _configBusniess.QueryAppConfigList(input);
        }
        /// <summary>
        /// 设置配置信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("/Config/SetAppConfigInfo")]
        public async Task<SetAppConfigInfoOutput> SetAppConfigInfo([FromBody] SetAppConfigInfoInput input)
        {
            return await _configBusniess.SetAppConfigInfo(input);
        }
        /// <summary>
        /// 推送配置中心命令
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("/Config/PublishCommand")]
        public async Task<PublishCommandOutput> PublishCommand([FromBody] PublishCommandInput input)
        {
            return await _configBusniess.PublishCommand(input);
        }
    }
}