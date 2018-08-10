using Bucket.Core;
using Bucket.OpenTracing;
using Bucket.Tracing;
using Bucket.Tracing.Extensions;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace Bucket.AspNetCore.Filters
{
    public class WebApiTracingFilterAttribute : Attribute, IResourceFilter
    {
        private static Regex _tbbrRegex = new Regex(@"\s*|\t|\r|\n", RegexOptions.IgnoreCase);
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
                        span.Tags.Add("http.response", _jsonHelper.SerializeObject(objectResult.Value));
                    }
                }
                else if (context.Result is ContentResult)
                {
                    var content = (context.Result as ContentResult).Content;
                    if (!string.IsNullOrWhiteSpace(content))
                        span.Tags.Add("http.response", _tbbrRegex.Replace(content, ""));
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
                    httpContext.Request.EnableRewind();
                    var initialBody = httpContext.Request.Body;
                    httpContext.Request.Body.Position = 0;
                    var content = new StreamReader(httpContext.Request.Body, Encoding.UTF8).ReadToEnd();
                    if(!string.IsNullOrWhiteSpace(content))
                        span.Tags.Add("http.request", _tbbrRegex.Replace(content, ""));
                    httpContext.Request.Body.Position = 0;
                    httpContext.Request.Body = initialBody;
                }
                span.Tags.UserId(httpContext.GetUserId())
                         .UserIp(httpContext.GetUserIp());
                _tracer.Tracer.SetEntrySpan(span);
            }
        }
        
    }
}
