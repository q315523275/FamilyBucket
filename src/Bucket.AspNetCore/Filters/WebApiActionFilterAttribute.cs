using Bucket.Buried;
using Bucket.Core;
using Bucket.Exceptions;
using Bucket.Values;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Bucket.AspNetCore.Filters
{
    public class WebApiActionFilterAttribute : ActionFilterAttribute
    {
        private readonly IBuriedContext _buriedContext;
        private readonly IJsonHelper _jsonHelper;
        private string ActionInput { get; set; }
        private string HttpUrl { get; set; }
        /// <summary>
        /// Action 过滤器
        /// </summary>
        public WebApiActionFilterAttribute(IBuriedContext buriedContext, IJsonHelper jsonHelper) {
            _buriedContext = buriedContext;
            _jsonHelper = jsonHelper;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var modelState = context.ModelState;
            if (!modelState.IsValid)
            {
                var message = string.Empty;
                //获取第一个错误提示
                foreach (var key in modelState.Keys)
                {
                    var state = modelState[key];
                    if (state.Errors.Any())
                    {
                        message = state.Errors.First().ErrorMessage;
                        break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(message))
                {
                    var errorInfo = _jsonHelper.DeserializeObject<ErrorResult>(message);
                    if (errorInfo != null)
                        throw new BucketException(errorInfo.ErrorCode, errorInfo.Message);
                }
            }
            // 埋点
            HttpUrl = new StringBuilder().Append(context.HttpContext.Request.Scheme)
                .Append("://")
                .Append(context.HttpContext.Request.Host)
                .Append(context.HttpContext.Request.PathBase)
                .Append(context.HttpContext.Request.Path)
                .Append(context.HttpContext.Request.QueryString)
                .ToString();
            if (context.HttpContext.Request.Method.ToLower() == HttpMethod.Post.ToString().ToLower())
            {
                using (var ms = new MemoryStream())
                {
                    try { context.HttpContext.Request.Body.Position = 0; } catch (Exception ex) { }
                    context.HttpContext.Request.Body.CopyTo(ms);
                    ms.Position = 0;
                    var myByteArray = ms.ToArray();
                    ActionInput = Encoding.UTF8.GetString(myByteArray);
                }
            }
            else if (context.HttpContext.Request.Method.ToLower() == HttpMethod.Get.ToString().ToLower())
            {
                ActionInput = context.HttpContext.Request.QueryString.Value;
            }
            _buriedContext.PublishAsync(new BuriedInformationInput { FuncName = context.HttpContext.Request.Path, ApiType = 1, ApiUri = HttpUrl, BussinessSuccess = 0, CalledResult = 0, InputParams = ActionInput }).GetAwaiter();
        }
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            var param = string.Empty;
            if (context.Result is ObjectResult)
            {
                var objectResult = context.Result as ObjectResult;
                if (objectResult.Value != null)
                {
                    param = _jsonHelper.SerializeObject(objectResult.Value);
                }
            }
            else if (context.Result is ContentResult)
            {
                param = (context.Result as ContentResult).Content;
            }
            _buriedContext.PublishAsync(new BuriedInformationInput { FuncName = context.HttpContext.Request.Path, ApiType = 1, ApiUri = HttpUrl, BussinessSuccess = 0, CalledResult = 0, InputParams = ActionInput, OutputParams = param }).GetAwaiter();
        }
    }
}
