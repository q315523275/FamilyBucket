using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Extensions
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// 使用配置中心获取host进行post json请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="configKey"></param>
        /// <param name="requestPath"></param>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PostAsJsonWithConfigAsync<T>(this HttpClient client, string configKey, string requestPath, T value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 使用服务发现获取host进行post json请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="serviceName"></param>
        /// <param name="requestPath"></param>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PostAsJsonWithBalancerAsync<T>(this HttpClient client, string serviceName, string requestPath, T value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
