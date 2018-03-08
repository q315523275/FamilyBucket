using Bucket.Buried;
using Bucket.Core;
using Bucket.LoadBalancer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace Bucket.ServiceClient.Http
{
    public class BucketHttpClient: IServiceClient
    {
        private readonly ILoadBalancerHouse _loadBalancerHouse;
        private readonly ILogger _logger;
        private readonly IBuriedContext _buriedContext;
        private readonly IJsonHelper _jsonHelper;
        private readonly HttpClient _httpClient;
        public BucketHttpClient(ILoadBalancerHouse loadBalancerHouse, ILoggerFactory loggerFactory, IBuriedContext buriedContext, IJsonHelper jsonHelper)
        {
            _logger = loggerFactory.CreateLogger<BucketHttpClient>();
            _loadBalancerHouse = loadBalancerHouse;
            _buriedContext = buriedContext;
            _jsonHelper = jsonHelper;
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
            bool isBuried = false)
        {
            #region 负载寻址
            var _load = _loadBalancerHouse.Get(serviceName).GetAwaiter().GetResult();
            if (_load == null)
                throw new ArgumentNullException(nameof(_load));
            var HostAndPort = _load.Lease().GetAwaiter().GetResult();
            if (HostAndPort == null)
                throw new ArgumentNullException(nameof(HostAndPort));
            string baseAddress = $"{scheme}://{HostAndPort.ToString()}";
            webApiPath = webApiPath.StartsWith("/") ? webApiPath : "/" + webApiPath;
            #endregion

            #region 下游请求头处理
            // 请求头下发，埋点请求头
            if (customHeaders == null) customHeaders = new Dictionary<string, string>();
            var downStreamHeaders = _buriedContext.DownStreamHeaders();
            // 合并键值
            customHeaders = customHeaders.Concat(downStreamHeaders).ToDictionary(k => k.Key, v => v.Value);
            #endregion

            #region 请求埋点
            if (isBuried) // 埋点
            {
                _buriedContext.PublishAsync(new
                {
                    ApiType = 1,
                    ApiUri = string.Concat(baseAddress, "/", webApiPath),
                    BussinessSuccess = 0,
                    CalledResult = 0,
                    InputParams = webApiPath
                }).GetAwaiter();
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
            var httpResponseMessage = _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult();
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK || httpResponseMessage.StatusCode == HttpStatusCode.NotModified)
            {
                var content = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                #region 请求埋点
                if (isBuried) // 埋点
                {
                    _buriedContext.PublishAsync(new
                    {
                        ApiType = 1,
                        ApiUri = string.Concat(baseAddress, "/", webApiPath),
                        BussinessSuccess = 0,
                        CalledResult = 0,
                        InputParams = webApiPath,
                        OutputParams = content,
                    }).GetAwaiter();
                }
                #endregion

                return _jsonHelper.DeserializeObject<T>(content);
            }
            #endregion

            throw new Exception($"服务{serviceName},路径{webApiPath}请求出错");
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
            bool isBuried = false) where T : class
        {
            #region 负载寻址
            var _load = _loadBalancerHouse.Get(serviceName).GetAwaiter().GetResult();
            if(_load == null)
                throw new ArgumentNullException(nameof(_load));
            var HostAndPort = _load.Lease().GetAwaiter().GetResult();
            if (HostAndPort == null)
                throw new ArgumentNullException(nameof(HostAndPort));
            string baseAddress = $"{scheme}://{HostAndPort.ToString()}";
            webApiPath = webApiPath.StartsWith("/") ? webApiPath : "/" + webApiPath;
            #endregion

            #region 下游请求头处理
            // 请求头下发
            if (customHeaders == null) customHeaders = new Dictionary<string, string>();
            var downStreamHeaders = _buriedContext.DownStreamHeaders();
            // 合并键值
            customHeaders = customHeaders.Concat(downStreamHeaders).ToDictionary(k => k.Key, v => v.Value);
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
            if(isBuried) // 埋点
            {
                _buriedContext.PublishAsync(new {
                    ApiType = 1,
                    ApiUri = string.Concat(baseAddress, "/", webApiPath),
                    BussinessSuccess = 0,
                    CalledResult = 0,
                    InputParams = body
                }).GetAwaiter();
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
            var httpResponseMessage = _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult();
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK || httpResponseMessage.StatusCode == HttpStatusCode.NotModified)
            {
                var content = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                #region 请求埋点
                if (isBuried) // 埋点
                {
                    _buriedContext.PublishAsync(new
                    {
                        ApiType = 1,
                        ApiUri = string.Concat(baseAddress, "/", webApiPath),
                        BussinessSuccess = 0,
                        CalledResult = 0,
                        InputParams = webApiPath,
                        OutputParams = content,
                    }).GetAwaiter();
                }
                #endregion

                return _jsonHelper.DeserializeObject<T>(content);
            }
            #endregion

            throw new Exception($"服务{serviceName},路径{webApiPath}请求出错");
        }
    }
}
