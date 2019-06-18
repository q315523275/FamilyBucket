using Bucket.Config.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bucket.Config.Configuration
{
    public class ConfigurationHelper
    {
        private readonly BucketConfigOptions _options;
        private readonly HttpClient client;
        public ConfigurationHelper(BucketConfigOptions options)
        {
            _options = options;
            client = new HttpClient();
        }
        public async Task<ConcurrentDictionary<string, string>> Get()
        {
            try
            {
                var apiurl = GetApiUrl();
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, apiurl));
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<ApiResult>(content);
                if (apiResult.ErrorCode == "000000" && apiResult.KV.Count > 0)
                {
                    return apiResult.KV;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return new ConcurrentDictionary<string, string>();
        }
        private string GetApiUrl()
        {
            string appId = _options.AppId;

            string secret = _options.AppSercet;

            var path = $"/configs/v2/{_options.AppId}/{_options.NamespaceName}";

            var query = $"version=0&env={_options.Env}";

            var sign = $"{query}&appId={appId}&appSecret={secret}&namespaceName={_options.NamespaceName}";

            var pathAndQuery = $"{path}?{query}&sign=" + SecureHelper.SHA256(sign);

            return $"{_options.ServerUrl.TrimEnd('/')}{pathAndQuery}";
        }
    }
}
