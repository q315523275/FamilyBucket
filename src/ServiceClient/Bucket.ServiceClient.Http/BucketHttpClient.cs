using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;


using Bucket.Core;
using Bucket.Tracer;
using Bucket.LoadBalancer;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Bucket.ServiceClient.Http
{
    public class BucketHttpClient: IServiceClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoadBalancerHouse _loadBalancerHouse;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILogger _logger;
        private readonly ITracerHandler _tracer;
        private readonly HttpClient _httpClient;
        public BucketHttpClient(ILoadBalancerHouse loadBalancerHouse, 
            ILoggerFactory loggerFactory, 
            ITracerHandler tracer, 
            IJsonHelper jsonHelper, 
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = loggerFactory.CreateLogger<BucketHttpClient>();
            _tracer = tracer;
            _jsonHelper = jsonHelper;
            _loadBalancerHouse = loadBalancerHouse;
            _httpContextAccessor = httpContextAccessor;

            _httpClient = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip })
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
        }

        /// <summary>
        /// get 请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="webApiPath"></param>
        /// <param name="scheme"></param>
        /// <param name="customHeaders"></param>
        /// <param name="MediaType"></param>
        /// <param name="isBuried"></param>
        /// <returns></returns>
        public T GetWebApi<T>(string serviceName, string webApiPath,
            string scheme = "http",
            Dictionary<string, string> customHeaders = null, 
            string MediaType = "application/json",
            bool isTrace = false)
        {
            #region 负载寻址
            var _load = _loadBalancerHouse.Get(serviceName).GetAwaiter().GetResult();
            if (_load == null)
                throw new ArgumentNullException(nameof(_load));
            var HostAndPort = _load.Lease().GetAwaiter().GetResult();
            if (HostAndPort == null)
                throw new ArgumentNullException(nameof(HostAndPort));
            string baseAddress = $"{scheme}://{HostAndPort.ToString()}/";
            webApiPath = webApiPath.TrimStart('/');
            #endregion

            #region 下游请求头处理
            if (isTrace)
            {
                // 请求头下发，埋点请求头
                if (customHeaders == null) customHeaders = new Dictionary<string, string>();
                var downStreamHeaders = _tracer.DownTraceHeaders(_httpContextAccessor.HttpContext);
                // 合并键值
                customHeaders = customHeaders.Concat(downStreamHeaders).ToDictionary(k => k.Key, v => v.Value);
            }
            #endregion

            #region 请求埋点
            var traceLog = new TraceLogs()
            {
                ApiUri = $"{baseAddress}{webApiPath}",
                ContextType = MediaType,
                StartTime = DateTime.Now,
            };
            if (isTrace)
            {
                _tracer.AddHeadersToTracer<TraceLogs>(_httpContextAccessor.HttpContext, traceLog);
                if (customHeaders.TryGetValue(TracerKeys.TraceSeq, out string value))
                {
                    traceLog.ParentSeq = value;
                }
            }
            #endregion

            #region http 请求
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseAddress}{webApiPath}"),
                Method = HttpMethod.Get
            };
            request.Headers.Clear();
            request.Headers.Accept.Clear();
            if (customHeaders != null)
            {
                foreach (KeyValuePair<string, string> customHeader in customHeaders)
                {
                    request.Headers.Add(customHeader.Key, customHeader.Value);
                }
            }
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            traceLog.Request = request.RequestUri.Query;
            try
            {
                var httpResponseMessage = _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult();
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    traceLog.IsSuccess = true;
                    traceLog.IsException = false;
                    if (typeof(T) == typeof(byte[]))
                    {
                        return (T)Convert.ChangeType(httpResponseMessage.Content.ReadAsByteArrayAsync().Result, typeof(T));
                    }
                    if (typeof(T) == typeof(Stream))
                    {
                        return (T)Convert.ChangeType(httpResponseMessage.Content.ReadAsStreamAsync().Result, typeof(T)); ;
                    }
                    if (typeof(T) == typeof(String))
                    {
                        var result = httpResponseMessage.Content.ReadAsStringAsync().Result;
                        traceLog.Response = result;
                        return (T)Convert.ChangeType(result, typeof(T));
                    }
                    else
                    {
                        var result = httpResponseMessage.Content.ReadAsStringAsync().Result;
                        traceLog.Response = result;
                        return _jsonHelper.DeserializeObject<T>(result);
                    }
                }
            }
            catch(Exception ex)
            {
                traceLog.IsException = true;
                _logger.LogError(ex, $"服务{serviceName},路径{webApiPath}接口请求异常");
            }
            finally
            {
                if (isTrace)
                {
                    traceLog.EndTime = DateTime.Now;
                    traceLog.TimeLength = Math.Round((traceLog.EndTime - traceLog.StartTime).TotalMilliseconds, 4);
                    _tracer.PublishAsync<TraceLogs>(traceLog).GetAwaiter();
                }
            }
            #endregion

            throw new Exception($"服务{serviceName},路径{webApiPath}接口请求异常");
        }
        /// <summary>
        /// post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="webApiPath"></param>
        /// <param name="obj"></param>
        /// <param name="scheme"></param>
        /// <param name="customHeaders"></param>
        /// <param name="MediaType"></param>
        /// <param name="encoder"></param>
        /// <param name="isBuried"></param>
        /// <returns></returns>
        public T PostWebApi<T>(string serviceName, string webApiPath, object obj,
            string scheme = "http",
            Dictionary<string, string> customHeaders = null, 
            string MediaType = "application/json", 
            Encoding encoder = null,
            bool isTrace = false) where T : class
        {
            #region 负载寻址
            var _load = _loadBalancerHouse.Get(serviceName).GetAwaiter().GetResult();
            if (_load == null)
                throw new ArgumentNullException(nameof(_load));
            var HostAndPort = _load.Lease().GetAwaiter().GetResult();
            if (HostAndPort == null)
                throw new ArgumentNullException(nameof(HostAndPort));
            string baseAddress = $"{scheme}://{HostAndPort.ToString()}/";
            webApiPath = webApiPath.TrimStart('/');
            #endregion

            #region 下游请求头处理
            if (isTrace)
            {
                // 请求头下发，埋点请求头
                if (customHeaders == null) customHeaders = new Dictionary<string, string>();
                var downStreamHeaders = _tracer.DownTraceHeaders(_httpContextAccessor.HttpContext);
                // 合并键值
                customHeaders = customHeaders.Concat(downStreamHeaders).ToDictionary(k => k.Key, v => v.Value);
            }
            #endregion

            #region http请求参数
            var body = string.Empty;
            if (MediaType == "application/json")
                body = _jsonHelper.SerializeObject(obj);
            else
            {
                var lst = new List<string>();
                foreach (var item in obj.GetType().GetRuntimeProperties())
                {
                    lst.Add(item.Name + "=" + item.GetValue(obj));
                }
                body = string.Join("&", lst.ToArray());
            }
            #endregion

            #region 请求埋点
            var traceLog = new TraceLogs()
            {
                ApiUri = $"{baseAddress}{webApiPath}",
                ContextType = MediaType,
                StartTime = DateTime.Now,
            };
            if (isTrace)
            {
                _tracer.AddHeadersToTracer<TraceLogs>(_httpContextAccessor.HttpContext, traceLog);
                if (customHeaders.TryGetValue(TracerKeys.TraceSeq, out string value))
                {
                    traceLog.ParentSeq = value;
                    traceLog.Request = body;
                }
            }
            #endregion

            #region http请求
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseAddress}{webApiPath}"),
                Method = HttpMethod.Post
            };
            request.Headers.Clear();
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            if (customHeaders != null)
            {
                foreach (KeyValuePair<string, string> customHeader in customHeaders)
                {
                    request.Headers.Add(customHeader.Key, customHeader.Value);
                }
            }
            request.Content = new StringContent(body, encoder ?? Encoding.UTF8, MediaType);
            try
            {
                var httpResponseMessage = _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult();
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK || httpResponseMessage.StatusCode == HttpStatusCode.NotModified)
                {
                    traceLog.IsSuccess = true;
                    traceLog.IsException = false;
                    if (typeof(T) == typeof(byte[]))
                    {
                        return (T)Convert.ChangeType(httpResponseMessage.Content.ReadAsByteArrayAsync().Result, typeof(T));
                    }
                    if (typeof(T) == typeof(Stream))
                    {
                        return (T)Convert.ChangeType(httpResponseMessage.Content.ReadAsStreamAsync().Result, typeof(T)); ;
                    }
                    if (typeof(T) == typeof(String))
                    {
                        var result = httpResponseMessage.Content.ReadAsStringAsync().Result;
                        traceLog.Response = result;
                        return (T)Convert.ChangeType(result, typeof(T));
                    }
                    else
                    {
                        var result = httpResponseMessage.Content.ReadAsStringAsync().Result;
                        traceLog.Response = result;
                        return _jsonHelper.DeserializeObject<T>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                traceLog.IsException = true;
                _logger.LogError(ex, $"服务{serviceName},路径{webApiPath}接口请求异常");
            }
            finally
            {
                if (isTrace)
                {
                    traceLog.EndTime = DateTime.Now;
                    traceLog.TimeLength = Math.Round((traceLog.EndTime - traceLog.StartTime).TotalMilliseconds, 4);
                    _tracer.PublishAsync<TraceLogs>(traceLog).GetAwaiter();
                }
            }
            #endregion

            throw new Exception($"服务{serviceName},路径{webApiPath}接口请求异常");
        }
    }
}
