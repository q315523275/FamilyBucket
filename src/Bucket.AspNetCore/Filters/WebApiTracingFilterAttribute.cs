using Bucket.Core;
using Bucket.OpenTracing;
using Bucket.Tracing;
using Bucket.Tracing.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Bucket.AspNetCore.Filters
{
    public class WebApiTracingFilterAttribute : Attribute, IResourceFilter
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly IServiceTracer _tracer;

        public WebApiTracingFilterAttribute(IJsonHelper jsonHelper, IServiceTracer tracer)
        {
            _jsonHelper = jsonHelper;
            _tracer = tracer;
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        { 
            var span = _tracer.Tracer.GetEntrySpan();
            if (span != null)
            {
                if (context.Result is ObjectResult)
                {
                    var objectResult = context.Result as ObjectResult;
                    if (objectResult.Value != null)
                    {
                        span.Tags.Add("response", _jsonHelper.SerializeObject(objectResult.Value));
                    }
                }
                else if (context.Result is ContentResult)
                {
                    span.Tags.Add("response", (context.Result as ContentResult).Content);
                }
                _tracer.Tracer.SetEntrySpan(span);
            }
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var span = _tracer.Tracer.GetEntrySpan();
            if (span != null)
            {
                var httpContext = context.HttpContext;
                if (httpContext.Request.Method.ToLower() == HttpMethod.Post.ToString().ToLower())
                {
                    if (httpContext.Request.Body.CanRead)
                    {
                        var memery = new MemoryStream();
                        httpContext.Request.Body.CopyTo(memery);
                        memery.Position = 0;
                        span.Tags.Add("request", new StreamReader(memery, Encoding.UTF8).ReadToEnd());
                        memery.Position = 0;
                        httpContext.Request.Body = memery;
                    }
                }

                if (httpContext.User.HasClaim(it => it.Type == "Uid"))
                    span.Tags.Add("uid", httpContext.User.Claims.FirstOrDefault(c => c.Type == "Uid").Value);

               _tracer.Tracer.SetEntrySpan(span);
            }
        }
    }
}
