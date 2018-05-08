using Bucket.Core;
using Bucket.Tracer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http.Headers;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
namespace Bucket.AspNetCore.Filters
{
    public class WebApiTraceFilterAttribute : Attribute, IResourceFilter
    {
        private readonly IRequestScopedDataRepository requestScopedDataRepository;
        private readonly IJsonHelper jsonHelper;
        public WebApiTraceFilterAttribute(IRequestScopedDataRepository requestScopedDataRepository, IJsonHelper jsonHelper)
        {
            this.requestScopedDataRepository = requestScopedDataRepository;
            this.jsonHelper = jsonHelper;
        }
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var trace = requestScopedDataRepository.Get<TraceLogs>(TracerKeys.TraceStoreCacheKey);
            if (trace != null)
            {
                if (context.HttpContext.Request.Method.ToLower() == HttpMethod.Post.ToString().ToLower())
                {
                    if (context.HttpContext.Request.Body.CanRead)
                    {
                        var memery = new MemoryStream();
                        context.HttpContext.Request.Body.CopyTo(memery);
                        memery.Position = 0;
                        trace.Request = new StreamReader(memery, Encoding.UTF8).ReadToEnd();
                        memery.Position = 0;
                        context.HttpContext.Request.Body = memery;
                    }
                }
                else if (context.HttpContext.Request.Method.ToLower() == HttpMethod.Get.ToString().ToLower())
                {
                    trace.Request = context.HttpContext.Request.QueryString.Value;
                }
                requestScopedDataRepository.Update(TracerKeys.TraceStoreCacheKey, trace);
            }
        }
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            var trace = requestScopedDataRepository.Get<TraceLogs>(TracerKeys.TraceStoreCacheKey);
            if (trace != null)
            {
                if (context.Result is ObjectResult)
                {
                    var objectResult = context.Result as ObjectResult;
                    if (objectResult.Value != null)
                    {
                        trace.Response = jsonHelper.SerializeObject(objectResult.Value);
                    }
                }
                else if (context.Result is ContentResult)
                {
                    trace.Response = (context.Result as ContentResult).Content;
                }
                trace.Code = "000000";
                trace.EndTime = DateTime.Now;
                trace.IsException = context.Exception != null;
                trace.IsSuccess = true;
                requestScopedDataRepository.Update(TracerKeys.TraceStoreCacheKey, trace);
            }
        }
    }
}
