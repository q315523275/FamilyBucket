using Bucket.ErrorCode;
using Bucket.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
namespace Bucket.AspNetCore.Middleware.Error
{
    public class ErrorLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IErrorCode _errorCodeStore;
        public ErrorLogMiddleware(RequestDelegate next,
            ILoggerFactory loggerFactory,
            IErrorCode errorCodeStore)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ErrorLogMiddleware>();
            _errorCodeStore = errorCodeStore;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BucketException ex)
            {
                var newMsg = _errorCodeStore.StringGet(ex.ErrorCode);
                if (string.IsNullOrWhiteSpace(newMsg))
                    newMsg = ex.ErrorMessage;
                if (string.IsNullOrWhiteSpace(newMsg))
                    newMsg = "未知异常,请稍后再试";

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json;charset=utf-8";
                await context.Response.WriteAsync("{\"ErrorCode\": \"" + ex.ErrorCode + "\", \"Message\": \"" + newMsg + "\"}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"全局异常捕获,状态码:{ context?.Response?.StatusCode},Url:{context?.Request?.GetDisplayUrl()}");

                // 开启异常,方便外层组件熔断等功能使用
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "text/plain;charset=utf-8";
                await context.Response.WriteAsync("error occurred");
            }
        }
    }
}
