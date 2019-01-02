using Bucket.Config.Abstractions;
using Bucket.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
namespace Bucket.Config.Implementation
{
    public class HttpDataRepository : IDataRepository
    {
        private readonly ILocalDataRepository _localDataRepository;
        private readonly IHttpUrlRepository _httpUrlRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpDataRepository> _logger;
        private readonly IJsonHelper _jsonHepler;
        private long _version = 0;

        public HttpDataRepository(ILocalDataRepository localDataRepository, IHttpUrlRepository httpUrlRepository, IHttpClientFactory httpClientFactory, IJsonHelper jsonHepler, ILogger<HttpDataRepository> logger)
        {
            _localDataRepository = localDataRepository;
            _httpUrlRepository = httpUrlRepository;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _jsonHepler = jsonHepler;
        }
        public ConcurrentDictionary<string, string> Data { get; private set; } = new ConcurrentDictionary<string, string>();
        public async Task Get(bool reload)
        {
            try
            {
                if (reload) _version = 0;
                var islocalcache = false;
                var apiurl = _httpUrlRepository.GetApiUrl(_version);
                var client = _httpClientFactory.CreateClient(); // 创建http请求
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, apiurl));
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var apiResult = _jsonHepler.DeserializeObject<ApiResult>(content);
                if (apiResult.ErrorCode == "000000" && apiResult.Version > _version)
                {
                    foreach (var kv in apiResult.KV)
                    {
                        Data.AddOrUpdate(kv.Key, kv.Value, (x, y) => kv.Value);
                    }
                    _version = apiResult.Version;
                    islocalcache = true;
                    // 注册数据更新监听通知
                    foreach(var _lits in DataChangeListenerDictionary.ToList())
                    {
                        _lits.OnDataChange(apiResult.KV);
                    }
                }
                if (islocalcache)
                    _localDataRepository.Set(Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"加载配置错误");
                foreach (var kv in _localDataRepository.Get())
                {
                    Data.AddOrUpdate(kv.Key, kv.Value, (x, y) => kv.Value);
                }
            }
        }
        public void AddChangeListener(IDataChangeListener dataChangeListener)
        {
            DataChangeListenerDictionary.Add(dataChangeListener);
        }
    }
}
