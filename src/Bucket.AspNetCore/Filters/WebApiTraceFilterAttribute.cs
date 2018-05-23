using Bucket.Core;
using Bucket.Tracer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
namespace Bucket.AspNetCore.Filters
{
    public class WebApiTraceFilterAttribute : Attribute, IResourceFilter
    {
        private readonly IServiceTracer _tracer;
        private readonly IJsonHelper jsonHelper;
        public WebApiTraceFilterAttribute(IServiceTracer tracer, IJsonHelper jsonHelper)
        {
            this._tracer = tracer;
            this.jsonHelper = jsonHelper;
        }
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var span = httpContext.GetSpan();
            if (span != null)
            {
                if (httpContext.Request.Method.ToLower() == HttpMethod.Post.ToString().ToLower())
                {
                    if (httpContext.Request.Body.CanRead)
                    {
                        var memery = new MemoryStream();
                        httpContext.Request.Body.CopyTo(memery);
                        memery.Position = 0;
                        span.Tags.RequstBody(new StreamReader(memery, Encoding.UTF8).ReadToEnd());
                        memery.Position = 0;
                        httpContext.Request.Body = memery;
                    }
                }
                if(string.IsNullOrWhiteSpace(span.LaunchId) && httpContext.User.HasClaim(it => it.Type == TracerKeys.TraceLaunchId))
                    span.LaunchId = httpContext.User.Claims.FirstOrDefault(c => c.Type == TracerKeys.TraceLaunchId).Value;
                httpContext.SetSpan(span);
            }
        }
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            var span = context.HttpContext.GetSpan();
            if (span != null)
            {
                if (context.Result is ObjectResult)
                {
                    var objectResult = context.Result as ObjectResult;
                    if (objectResult.Value != null)
                    {
                        span.Tags.ResponseBody(jsonHelper.SerializeObject(objectResult.Value));
                    }
                }
                else if (context.Result is ContentResult)
                {
                    span.Tags.ResponseBody((context.Result as ContentResult).Content);
                }
                context.HttpContext.SetSpan(span);
            }
        }
    }
}
