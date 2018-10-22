using Bucket.Core;
using Bucket.OpenTracing;
using Bucket.Tracing;
using Bucket.Tracing.Extensions;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Bucket.AspNetCore.Filters
{
    public class WebApiTracingFilterAttribute : Attribute, IResourceFilter
    {
        private static Regex _tbbrRegex = new Regex(@"\s*|\t|\r|\n", RegexOptions.IgnoreCase);
        private readonly IJsonHelper _jsonHelper;
        private readonly IServiceTracer _tracer;
        private readonly TracingOptions _options;
        public WebApiTracingFilterAttribute(IJsonHelper jsonHelper, IServiceTracer tracer, IOptions<TracingOptions> options)
        {
            _jsonHelper = jsonHelper;
            _tracer = tracer;
            _options = options.Value;
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            if (_options.TraceHttpContent)
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
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (_options.TraceHttpContent)
            {
                var span = _tracer.Tracer.GetEntrySpan();
                if (span != null)
                {
                    var bodyStr = string.Empty; ;
                    var req = context.HttpContext.Request;
                    if (req.Method.ToLower() == HttpMethod.Post.ToString().ToLower())
                    {
                        req.EnableRewind();
                        var originBody = req.Body;
                        req.Body.Position = 0;
                        bodyStr = new StreamReader(req.Body).ReadToEnd();
                        req.Body.Position = 0;
                        req.Body = originBody;
                        if (!string.IsNullOrWhiteSpace(bodyStr))
                            span.Tags.Add("http.request", _tbbrRegex.Replace(bodyStr, ""));
                    }
                    span.Tags.UserId(context.HttpContext.GetUserId())
                             .UserIp(context.HttpContext.GetUserIp());
                    _tracer.Tracer.SetEntrySpan(span);
                }
            }
        }
    }
}
