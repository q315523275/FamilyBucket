using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bucket.ServiceClient
{
    /// <summary>
    /// 服务请求（需服务发现、埋点、负载算法、）
    /// </summary>
    public interface IServiceClient
    {
        /// <summary>
        /// Api Post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="webApiPath"></param>
        /// <param name="obj"></param>
        /// <param name="customHeaders"></param>
        /// <param name="MediaType"></param>
        /// <param name="encoder"></param>
        /// <param name="scheme"></param>
        /// <param name="isTrace"></param>
        /// <returns></returns>
        T PostWebApi<T>(string serviceName, string webApiPath, Object obj,
                                            string scheme = "http",
                                            Dictionary<string, string> customHeaders = null, 
                                            string MediaType = "application/json", 
                                            Encoding encoder = null,
                                            bool isTrace = false) where T : class;
        /// <summary>
        /// Api Get请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="webApiPath"></param>
        /// <param name="customHeaders"></param>
        /// <param name="MediaType"></param>
        /// <param name="scheme"></param>
        /// <param name="isTrace"></param>
        /// <returns></returns>
        T GetWebApi<T>(string serviceName, string webApiPath,
                                           string scheme = "http",
                                           Dictionary<string, string> customHeaders = null, 
                                           string MediaType = "application/json",
                                           bool isTrace = false);
    }
}
