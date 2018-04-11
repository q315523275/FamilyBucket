using Bucket.EventBus.Common.Events;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Bucket.Values;
using System.Security.Claims;
using Bucket.Core;

namespace Bucket.Buried
{
    /// <summary>
    /// 埋点处理程序
    /// </summary>
    public class BuriedContext : IBuriedContext
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<BuriedContext> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IJsonHelper _jsonHelper;
        private HttpContext _httpContext { get; set; }
        private readonly string BuriedIndex = "BuriedIndex";
        private readonly string ProjectSeq = "PZGOSeq";
        public BuriedContext(IEventBus eventBus, 
            ILogger<BuriedContext> logger, 
            IHttpContextAccessor httpContextAccessor, 
            IJsonHelper jsonHelper)
        {
            _eventBus = eventBus;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _jsonHelper = jsonHelper;
        }
        /// <summary>
        /// 埋点推送
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buriedInformation"></param>
        /// <returns></returns>
        public async Task PublishAsync<T>(T buriedInformation)
        {
            _httpContext = _httpContextAccessor.HttpContext;
            await EventPublishAsync<T>(buriedInformation);
        }
        /// <summary>
        /// 获取请求头值
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetValue(Dictionary<string, string> headers, string key)
        {
            return headers.ContainsKey(key) ? headers[key] : string.Empty;
        }
        /// <summary>
        /// 时间戳转换
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, dateTime.Kind);
            return Convert.ToInt64((dateTime.AddHours(-8) - start).TotalSeconds * 1000);
        }
        /// <summary>
        /// 上游请求头信息
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> UpStreamHeaders()
        {
            var headers = new Dictionary<string, string>();
            // 项目名称
            if (_httpContext.Request.Headers.ContainsKey("ModName"))
                headers.Add("ModName", _httpContext.Request.Headers["ModName"].FirstOrDefault());
            // 流程名称
            if (_httpContext.Request.Headers.ContainsKey("ProcName"))
                headers.Add("ProcName", _httpContext.Request.Headers["ProcName"].FirstOrDefault());
            // 流程发起人
            if (_httpContext.Request.Headers.ContainsKey("LaunchId"))
                headers.Add("LaunchId", _httpContext.Request.Headers["LaunchId"].FirstOrDefault());
            // 流程Token
            if (_httpContext.Request.Headers.ContainsKey("Token"))
                headers.Add("Token", _httpContext.Request.Headers["Token"].FirstOrDefault());
            // BatchId
            if (_httpContext.Request.Headers.ContainsKey("BatchId"))
                headers.Add("BatchId", _httpContext.Request.Headers["BatchId"].FirstOrDefault());
            // 当前队列顺序号
            if (_httpContext.Request.Headers.ContainsKey("Seq"))
                headers.Add("Seq", _httpContext.Request.Headers["Seq"].FirstOrDefault());
            return headers;
        }
        /// <summary>
        /// 下游请求头转发
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> DownStreamHeaders()
        {
            var headers = UpStreamHeaders();
            headers.Remove("Seq");
            // 当前队列顺序号
            if (_httpContext.Items.ContainsKey(ProjectSeq)
                && !string.IsNullOrWhiteSpace(_httpContext.Items[ProjectSeq].ToString()))
            {
                headers.Add("Seq", _httpContext.Items[ProjectSeq].ToString());
            }
            // token续传
            if (_httpContext.Request.Headers.ContainsKey("Authorization"))
                headers.Add("Authorization", _httpContext.Request.Headers["Authorization"].FirstOrDefault());
            return headers;
        }
        /// <summary>
        /// 埋点推送
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buriedInformation"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task PublishAsync<T>(T buriedInformation, HttpContext httpContext)
        {
            _httpContext = httpContext;
            await EventPublishAsync<T>(buriedInformation);
        }
        /// <summary>
        /// 埋点事件推送
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buriedInformation"></param>
        /// <returns></returns>
        private async Task EventPublishAsync<T>(T buriedInformation)
        {
            var pheader = DownStreamHeaders();
            if (buriedInformation != null
                && _httpContext != null
                && pheader.Count > 0)
            {
                var logInfo = (BuriedInformationInput)Convert.ChangeType(buriedInformation, typeof(BuriedInformationInput));
                // 父级信息
                var buriedInfo = new BuriedInformation()
                {
                    ModName = GetValue(pheader, "ModName"),
                    ProcName = GetValue(pheader, "ProcName"),
                    LaunchId = GetValue(pheader, "LaunchId"),
                    Token = GetValue(pheader, "Token"),
                    BatchId = GetValue(pheader, "BatchId"),
                    Seq = GetValue(pheader, "Seq")
                };

                // 父级
                if (string.IsNullOrWhiteSpace(buriedInfo.Seq))
                    buriedInfo.ParentSeq = "-1";
                else
                    buriedInfo.ParentSeq = buriedInfo.Seq;
                // 请求头
                if (string.IsNullOrWhiteSpace(logInfo.HeaderParams)
                    && _httpContext.User.Identity.IsAuthenticated)
                {
                    var dic = new Dictionary<string, object>{
                        { "Uid", _httpContext.User.Claims.First(c => c.Type == "Uid").Value },
                        { "RealName", _httpContext.User.Claims.First(c => c.Type == "Name").Value },
                        { "Mobile", _httpContext.User.Claims.First(c => c.Type == "MobilePhone").Value }
                    };
                    logInfo.HeaderParams = _jsonHelper.SerializeObject(dic);
                }
                // 索引
                var index = 0;
                if (_httpContext.Items.ContainsKey(BuriedIndex))
                    index = Convert.ToInt32(_httpContext.Items[BuriedIndex]);
                else
                    _httpContext.Items.Add(BuriedIndex, 0);

                // 埋点赋值
                buriedInfo.Seq = string.Concat("PZGO_" + index);
                buriedInfo.BuriedTime = DateTimeToUnixTimestamp(DateTime.Now).ToString();
                buriedInfo.ApiType = logInfo.ApiType;
                buriedInfo.ApiUri = logInfo.ApiUri;
                buriedInfo.BussinessDesc = logInfo.BussinessDesc;
                buriedInfo.BussinessSuccess = logInfo.BussinessSuccess;
                buriedInfo.CalledResult = logInfo.CalledResult;
                buriedInfo.Description = logInfo.Description;
                buriedInfo.FuncName = logInfo.FuncName;
                buriedInfo.HeaderParams = logInfo.HeaderParams;
                buriedInfo.InputParams = logInfo.InputParams;
                buriedInfo.TrgAlarm = logInfo.TrgAlarm;
                buriedInfo.OutputParams = logInfo.OutputParams;
                buriedInfo.Source = "PZGO";
                buriedInfo.ErrorCode = logInfo.ErrorCode;

                _httpContext.Items[ProjectSeq] = buriedInfo.Seq; // 给下游准备请求头
                _httpContext.Items[BuriedIndex] = index + 1; // 索引编号

                await _eventBus.PublishAsync(new BuriedEvent(buriedInfo));
            }
        }
    }
}
