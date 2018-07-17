using Bucket.Config.Exceptions;
using Bucket.Core;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bucket.Config.Util.Http
{
    public static class HttpUtil
    {
        private static HttpClient _httpClient = new HttpClient();
        static HttpUtil()
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
        }
        public static async Task<HttpResponse<T>> Get<T>(HttpRequest httpRequest, IJsonHelper jsonHelper)
        {
            try
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(httpRequest.Url),
                    Method = HttpMethod.Get,
                };
                var response = await _httpClient.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotModified)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    T body = jsonHelper.DeserializeObject<T>(content);
                    return new HttpResponse<T>(response.StatusCode, body);
                }
                throw new RemoteStatusCodeException(response.StatusCode, string.Format("Get operation failed for {0}", httpRequest.Url));
            }
            catch (Exception ex)
            {
                throw new RemoteException("Could not complete get operation", ex);
            }
        }
    }
}
