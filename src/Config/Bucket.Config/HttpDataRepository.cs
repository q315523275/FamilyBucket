using Bucket.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bucket.Config
{
    public class HttpDataRepository : IDataRepository
    {
        private readonly ILocalDataRepository _localDataRepository;
        private readonly IHttpUrlRepository _httpUrlRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILogger<HttpDataRepository> _logger;
        private long _version = 0;

        public HttpDataRepository(ILocalDataRepository localDataRepository, IHttpUrlRepository httpUrlRepository, IHttpClientFactory httpClientFactory, IJsonHelper jsonHelper, ILogger<HttpDataRepository> logger)
        {
            _localDataRepository = localDataRepository;
            _httpUrlRepository = httpUrlRepository;
            _httpClientFactory = httpClientFactory;
            _jsonHelper = jsonHelper;
            _logger = logger;
        }

        public ConcurrentDictionary<string, string> Data { get; private set; } = new ConcurrentDictionary<string, string>();
        public async Task Get(bool reload)
        {
            try
            {
                if (reload) _version = 0;
                var islocalcache = false;
                var apiurl = await _httpUrlRepository.GetApiUrl(_version);
                var client = _httpClientFactory.CreateClient(); // 创建http请求
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, apiurl));
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var output = _jsonHelper.DeserializeObject<ApiOutput>(content);
                if (output.ErrorCode == "000000" && output.Version > _version)
                {
                    foreach (var kv in output.KV)
                    {
                        Data.AddOrUpdate(kv.Key, kv.Value, (x, y) => kv.Value);
                    }
                    _version = output.Version;
                    islocalcache = true;
                }
                if (islocalcache)
                    _localDataRepository.Set(Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"加载配置错误");
                Data = _localDataRepository.Get();
            }
        }
    }
}
