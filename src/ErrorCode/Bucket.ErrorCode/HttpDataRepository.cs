using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bucket.Core;
using Bucket.ErrorCode.Model;
using Bucket.ErrorCode.Utils;
using Microsoft.Extensions.Logging;

namespace Bucket.ErrorCode
{
    public class HttpDataRepository : IDataRepository
    {
        private readonly ILocalDataRepository _localDataRepository;
        private readonly IHttpUrlRepository _httpUrlRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILogger<HttpDataRepository> _logger;
        private readonly ThreadSafe.AtomicReference<IList<ApiErrorCodeInfo>> _dataList;
        public HttpDataRepository(ILocalDataRepository localDataRepository, IHttpUrlRepository httpUrlRepository, IHttpClientFactory httpClientFactory, IJsonHelper jsonHelper, ILogger<HttpDataRepository> logger)
        {
            _localDataRepository = localDataRepository;
            _httpUrlRepository = httpUrlRepository;
            _httpClientFactory = httpClientFactory;
            _jsonHelper = jsonHelper;
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
                var output = _jsonHelper.DeserializeObject<ApiInfo>(content);
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
