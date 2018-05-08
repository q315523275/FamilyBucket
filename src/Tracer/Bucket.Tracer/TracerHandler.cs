using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Bucket.Core;
using System.Linq;
using System.Collections.Generic;

namespace Bucket.Tracer
{
    public class TracerHandler : ITracerHandler
    {
        private readonly IRequestScopedDataRepository _requestScopedDataRepository;
        private readonly ITracerStore _tracerStore;
        private readonly IJsonHelper _jsonHelper;
        public TracerHandler(IRequestScopedDataRepository requestScopedDataRepository, IJsonHelper jsonHelper, ITracerStore tracerStore)
        {
            _requestScopedDataRepository = requestScopedDataRepository;
            _tracerStore = tracerStore;
            _jsonHelper = jsonHelper;
        }

        public void AddHeadersToTracer<T>(HttpContext httpContext, T traceLogs)
        {
            if (traceLogs.GetType().Equals(typeof(TraceLogs)))
            {
                var trace = traceLogs as TraceLogs;

                #region 系统参数
                trace.Environment = TracerKeys.Environment;
                trace.SystemName = TracerKeys.SystemName;
                #endregion

                #region 跟踪Id
                if (string.IsNullOrWhiteSpace(trace.TraceId))
                {
                    if (httpContext.Request.Headers.ContainsKey(TracerKeys.TraceId))
                    {
                        trace.TraceId = httpContext.Request.Headers[TracerKeys.TraceId].FirstOrDefault();
                    }
                    else
                    {
                        trace.TraceId = Guid.NewGuid().ToString("N");
                        httpContext.Request.Headers.Add(TracerKeys.TraceId, trace.TraceId);
                    }
                }
                #endregion

                #region 用户标识
                if (string.IsNullOrWhiteSpace(trace.LaunchId) && httpContext.User.HasClaim(it => it.Type == TracerKeys.TraceLaunchId))
                {
                    trace.LaunchId = httpContext.User.Claims.First(c => c.Type == TracerKeys.TraceLaunchId).Value;
                }
                #endregion

                #region 项目标识
                if (string.IsNullOrWhiteSpace(trace.ModName) && httpContext.Request.Headers.ContainsKey(TracerKeys.TraceModName))
                {
                    trace.ModName = httpContext.Request.Headers[TracerKeys.TraceModName].FirstOrDefault();
                }
                else
                {
                    trace.ModName = trace.SystemName;
                }
                #endregion

                #region 父级序列号
                if (string.IsNullOrWhiteSpace(trace.ParentSeq))
                {
                    if (httpContext.Request.Headers.ContainsKey(TracerKeys.TraceSeq))
                    {
                        trace.ParentSeq = httpContext.Request.Headers[TracerKeys.TraceSeq].FirstOrDefault();
                    }
                    else
                    {
                        trace.ParentSeq = "-1";
                    }
                }
                #endregion

                #region 当前序列号
                int traceSerial = _requestScopedDataRepository.Get<int>(TracerKeys.TraceSerial);
                traceSerial = traceSerial + 1;
                trace.Seq = $"{trace.SystemName}_{traceSerial}";
                _requestScopedDataRepository.Update<int>(TracerKeys.TraceSerial, traceSerial);
                _requestScopedDataRepository.Update(TracerKeys.TraceSeq, trace.Seq);
                #endregion
            }
        }

        public Dictionary<string, string> DownTraceHeaders(HttpContext httpContext)
        {
            var kv = new Dictionary<string, string>();

            #region 跟踪Id
            if (httpContext.Request.Headers.ContainsKey(TracerKeys.TraceId))
            {
                kv.Add(TracerKeys.TraceId, httpContext.Request.Headers[TracerKeys.TraceId].FirstOrDefault());
            }
            #endregion

            #region 用户标识
            if (httpContext.User.HasClaim(it => it.Type == TracerKeys.TraceLaunchId))
            {
                kv.Add(TracerKeys.TraceLaunchId, httpContext.User.Claims.First(c => c.Type == TracerKeys.TraceLaunchId).Value);
            }
            #endregion

            #region 项目标识
            if (httpContext.Request.Headers.ContainsKey(TracerKeys.TraceModName))
            {
                kv.Add(TracerKeys.TraceModName, httpContext.Request.Headers[TracerKeys.TraceModName].FirstOrDefault());
            }
            #endregion
            
            #region 当前标识
            kv.Add(TracerKeys.TraceSeq, _requestScopedDataRepository.Get<string>(TracerKeys.TraceSeq));
            #endregion

            #region 认证
            if (httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                kv.Add("Authorization", httpContext.Request.Headers["Authorization"].FirstOrDefault());
            }
            #endregion

            return kv;
        }

        public async Task PublishAsync<T>(T traceLogs)
        {
            if(traceLogs.GetType().Equals(typeof(TraceLogs)))
            {
                await _tracerStore.Post((traceLogs as TraceLogs));
            }
        }
    }
}
