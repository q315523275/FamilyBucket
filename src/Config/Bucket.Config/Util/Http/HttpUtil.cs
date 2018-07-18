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
        public static HttpResponse<T> Get<T>(HttpRequest httpRequest, IJsonHelper jsonHelper)
        {
            try
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(httpRequest.Url),
                    Method = HttpMethod.Get,
                };
                var response = _httpClient.SendAsync(request).Result;
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotModified)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    T body = jsonHelper.DeserializeObject<T>(content);
                    return new HttpResponse<T>(response.StatusCode, body);
                }
                throw new Exception($"Get operation failed for {httpRequest.Url}, status {response.StatusCode}");
            }
            catch (Exception ex)
            {
                throw new Exception("Could not complete get operation", ex);
            }
        }
    }
}
