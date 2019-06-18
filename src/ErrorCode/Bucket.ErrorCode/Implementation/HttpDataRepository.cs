using Bucket.ErrorCode.Abstractions;
using Bucket.ErrorCode.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bucket.ErrorCode.Implementation
{
    public class HttpDataRepository : IDataRepository
    {
        private readonly ILocalDataRepository _localDataRepository;
        private readonly IHttpUrlRepository _httpUrlRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpDataRepository> _logger;
        private readonly ThreadSafe.AtomicReference<IList<ApiErrorCodeInfo>> _dataList;
        public HttpDataRepository(ILocalDataRepository localDataRepository, IHttpUrlRepository httpUrlRepository, IHttpClientFactory httpClientFactory, ILogger<HttpDataRepository> logger)
        {
            _localDataRepository = localDataRepository;
            _httpUrlRepository = httpUrlRepository;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _dataList = new ThreadSafe.AtomicReference<IList<ApiErrorCodeInfo>>(new List<ApiErrorCodeInfo>());
        }

        public IList<ApiErrorCodeInfo> Data { get { return _dataList.ReadFullFence(); } }

        public async Task Get()
        {
            try
            {
                var islocalcache = false;
                var apiurl = _httpUrlRepository.GetApiUrl();
                var client = _httpClientFactory.CreateClient();
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, apiurl));
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var output = JsonConvert.DeserializeObject<ApiInfo>(content);
                if (output != null && output.Value != null && output.Value.Count > 0)
                {
                    _dataList.WriteFullFence(output.Value);
                    islocalcache = true;
                }
                if (islocalcache)
                    _localDataRepository.Set(_dataList.ReadFullFence());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"加载配置错误");
                _dataList.WriteFullFence(_localDataRepository.Get());
            }
        }
    }
}
