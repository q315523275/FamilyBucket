using Bucket.Core;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bucket.ErrorCode.Util.Http
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
                var result = await _httpClient.SendAsync(request);
                if (result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.NotModified)
                {
                    var content = result.Content.ReadAsStringAsync().Result;
                    T body = jsonHelper.DeserializeObject<T>(content);
                    return new HttpResponse<T>(result.StatusCode, body);
                }
                throw new Exception(string.Format("Get operation failed for {0}", httpRequest.Url));
            }
            catch (Exception ex)
            {
                throw new Exception("Could not complete get operation", ex);
            }
        }
    }
}
