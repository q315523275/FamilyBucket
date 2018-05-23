using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;


using Bucket.Core;
using Bucket.LoadBalancer;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Bucket.ServiceClient.Http
{
    public class BucketHttpClient: IServiceClient
    {
        private readonly ILoadBalancerHouse _loadBalancerHouse;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        public BucketHttpClient(ILoadBalancerHouse loadBalancerHouse, 
            ILoggerFactory loggerFactory, 
            IJsonHelper jsonHelper, 
            IHttpContextAccessor httpContextAccessor,
            HttpClient httpClient)
        {
            _logger = loggerFactory.CreateLogger<BucketHttpClient>();
            _jsonHelper = jsonHelper;
            _loadBalancerHouse = loadBalancerHouse;

            _httpClient = httpClient ?? new HttpClient();
            //_httpClient.Timeout = TimeSpan.FromSeconds(30);
            //_httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
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
        private T ToResult<T>(HttpResponseMessage httpResponseMessage)
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
                return (T)Convert.ChangeType(result, typeof(T));
            }
            else
            {
                var result = httpResponseMessage.Content.ReadAsStringAsync().Result;
                return _jsonHelper.DeserializeObject<T>(result);
            }
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
            string MediaType = "application/json")
        {
            // 接口地址
            var apiUri = GetApiUrl(serviceName, webApiPath, scheme);
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
            try
            {
                var httpResponseMessage = _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult();
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return ToResult<T>(httpResponseMessage);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"服务{serviceName},路径{webApiPath}接口请求异常");
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
            Encoding encoder = null)
        {
            // 接口地址
            var apiUri = GetApiUrl(serviceName, webApiPath, scheme);
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
                    return ToResult<T>(httpResponseMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"服务{serviceName},路径{webApiPath}接口请求异常");
            }
            throw new Exception($"服务{serviceName},路径{webApiPath}接口请求异常");
        }
    }
}
