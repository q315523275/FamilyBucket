
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using Nito.AsyncEx;
using System.Threading.Tasks;
using Bucket.Config.Abstractions;
using Bucket.Config.Implementation;
using Bucket.Config.Utils;
using Newtonsoft.Json;
namespace Bucket.Config.Configuration
{
    /// <summary>
    /// 翻看底层源码,暂未找到合理方案,除非改变整体结构
    /// </summary>
    public class BucketConfigurationProvider : ConfigurationProvider, IDataChangeListener, IConfigurationSource
    {
        private readonly BucketConfigOptions _options;
        public BucketConfigurationProvider(BucketConfigOptions options)
        {
            _options = options;
            Data = new ConcurrentDictionary<string, string>();
        }

        public override void Load()
        {
            DataChangeListenerDictionary.Add(this);

            AsyncContext.Run(() => Get());
        }

        private void SetData(ConcurrentDictionary<string, string> changeData)
        {
            foreach(var dic in changeData)
            {
                if (Data.ContainsKey(dic.Key))
                    Data[dic.Key] = dic.Value;
                else
                    Data.Add(dic);
            }
            // Data = new Dictionary<string, string>(_configRepository.Data, StringComparer.OrdinalIgnoreCase);
        }

        public void OnDataChange(ConcurrentDictionary<string, string> changeData)
        {
            SetData(changeData);
            OnReload();
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => this;

        // 以下临时解决办法
        private async Task Get()
        {
            try
            {
                var apiurl = GetApiUrl();
                var client = new HttpClient(); // 创建http请求
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, apiurl));
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<ApiResult>(content);
                if (apiResult.ErrorCode == "000000" && apiResult.KV.Count > 0)
                {
                    Data = apiResult.KV;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public string GetApiUrl()
        {
            string appId = _options.AppId;

            string secret = _options.AppSercet;

            var path = $"/configs/{_options.AppId}/{_options.NamespaceName}";

            var query = $"version=0";

            var sign = $"appId={appId}&appSecret={secret}&namespaceName={_options.NamespaceName}";

            var pathAndQuery = $"{path}?{query}&env={_options.Env}&sign=" + SecureHelper.SHA256(sign);

            return $"{_options.ServerUrl.TrimEnd('/')}{pathAndQuery}";
        }
    }
}
