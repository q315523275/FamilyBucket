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
        /// 获取负载Api接口地址
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="webApiPath"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        private string GetApiUrl(string serviceName, string webApiPath, string scheme)
        {
            var _load = _loadBalancerHouse.Get(serviceName).GetAwaiter().GetResult();
            if (_load == null)
                throw new ArgumentNullException(nameof(_load));
            var HostAndPort = _load.Lease().GetAwaiter().GetResult();
            if (HostAndPort == null)
                throw new ArgumentNullException(nameof(HostAndPort));
            string baseAddress = $"{scheme}://{HostAndPort.ToString()}/";
            webApiPath = webApiPath.TrimStart('/');

            return $"{baseAddress}{webApiPath}";
        }
        /// <summary>
        /// http请求结果转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpResponseMessage"></param>
        /// <param name="traceLogs"></param>
        /// <returns></returns>
        private T ToResult<T>(HttpResponseMessage httpResponseMessage, TraceLogs traceLogs)
        {
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
                traceLogs.Response = result;
                return (T)Convert.ChangeType(result, typeof(T));
            }
            else
            {
                var result = httpResponseMessage.Content.ReadAsStringAsync().Result;
                traceLogs.Response = result;
                return _jsonHelper.DeserializeObject<T>(result);
            }
        }
        /// <summary>
        /// 创建追踪基础数据
        /// </summary>
        /// <param name="apiUri"></param>
        /// <param name="mediaType"></param>
        /// <param name="isTrace"></param>
        /// <param name="customHeaders"></param>
        /// <returns></returns>
        private TraceLogs CreateBaseTracer(string apiUri, string mediaType, bool isTrace, Dictionary<string, string> customHeaders)
        {
            if (isTrace)
            {
                // 请求头下发，埋点请求头
                if (customHeaders == null) customHeaders = new Dictionary<string, string>();
                var downStreamHeaders = _tracer.DownTraceHeaders(_httpContextAccessor.HttpContext);
                // 合并键值
                customHeaders = customHeaders.Concat(downStreamHeaders).ToDictionary(k => k.Key, v => v.Value);
            }
            var traceLog = new TraceLogs()
            {
                ApiUri = apiUri,
                ContextType = mediaType,
                StartTime = DateTime.Now,
            };
            if (isTrace)
            {
                _tracer.AddHeadersToTracer<TraceLogs>(_httpContextAccessor.HttpContext, traceLog);
            }
            return traceLog;
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
            // 接口地址
            var apiUri = GetApiUrl(serviceName, webApiPath, scheme);
            // 追踪信息
            var traceLog = CreateBaseTracer(apiUri, MediaType, isTrace, customHeaders);
            // http请求
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(apiUri),
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
                    return ToResult<T>(httpResponseMessage, traceLog);
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
                    _tracer.PublishAsync(traceLog).GetAwaiter();
                }
            }
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
            bool isTrace = false)
        {
            // 接口地址
            var apiUri = GetApiUrl(serviceName, webApiPath, scheme);
            // 追踪信息
            var traceLog = CreateBaseTracer(apiUri, MediaType, isTrace, customHeaders);
            // post内容
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
            traceLog.Request = body;
            // http请求
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(apiUri),
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
                    return ToResult<T>(httpResponseMessage, traceLog);
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
            throw new Exception($"服务{serviceName},路径{webApiPath}接口请求异常");
        }


        /// <summary>
        /// get 请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiUri"></param>
        /// <param name="customHeaders"></param>
        /// <param name="MediaType"></param>
        /// <param name="isBuried"></param>
        /// <returns></returns>
        public T GetWebApi<T>(string apiUri,
            Dictionary<string, string> customHeaders = null,
            string MediaType = "application/json",
            bool isTrace = false)
        {
            // 追踪信息
            var traceLog = CreateBaseTracer(apiUri, MediaType, isTrace, customHeaders);

            #region http 请求
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(apiUri),
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
                    return ToResult<T>(httpResponseMessage, traceLog);
                }
            }
            catch (Exception ex)
            {
                traceLog.IsException = true;
                _logger.LogError(ex, $"接口{apiUri}请求异常");
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

            throw new Exception($"接口{apiUri}请求异常");
        }

        /// <summary>
        /// post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiUri"></param>
        /// <param name="obj"></param>
        /// <param name="customHeaders"></param>
        /// <param name="MediaType"></param>
        /// <param name="encoder"></param>
        /// <param name="isBuried"></param>
        /// <returns></returns>

        public T PostWebApi<T>(string apiUri, object obj,
            Dictionary<string, string> customHeaders = null,
            string MediaType = "application/json",
            Encoding encoder = null,
            bool isTrace = false)
        {
            // 追踪信息
            var traceLog = CreateBaseTracer(apiUri, MediaType, isTrace, customHeaders);
            // post内容
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
            traceLog.Request = body;

            // http请求
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(apiUri),
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
                    return ToResult<T>(httpResponseMessage, traceLog);
                }
            }
            catch (Exception ex)
            {
                traceLog.IsException = true;
                _logger.LogError(ex, $"接口{apiUri}请求异常");
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

            throw new Exception($"接口{apiUri}请求异常");
        }
    }
}
