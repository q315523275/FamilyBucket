using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Bucket.Config.Util;
using Bucket.Redis;
using Bucket.Core;
using System.Collections.Concurrent;
using StackExchange.Redis;
using System.Net.Http;

namespace Bucket.Config
{
    /// <summary>
    /// 远程配置仓储
    /// </summary>
    public class RemoteConfigRepository
    {
        private BucketConfig _config;
        private ConfigServiceLocator _serviceLocator; // 配置拉取地址方法
        private readonly ConfigOptions _setting;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private bool initialized = false; // 是否初始化配置
        private static readonly object _lock = new object();
        public RemoteConfigRepository(ConfigOptions setting,
            RedisClient redisClient,
            ConfigServiceLocator configServiceLocator,
            ILoggerFactory loggerFactory,
            IJsonHelper jsonHelper,
            IHttpClientFactory httpClientFactory)
        {
            _logger = loggerFactory.CreateLogger<RemoteConfigRepository>();
            _config = new BucketConfig();

            _setting = setting;

            _serviceLocator = configServiceLocator;

            _jsonHelper = jsonHelper;

            _httpClientFactory = httpClientFactory;
        }
        /// <summary>
        /// 配置键值返回
        /// </summary>
        /// <returns></returns>
        public ConcurrentDictionary<string, string> GetConfig()
        {
            // 未初始化参数
            if (!initialized)
            {
                lock (_lock)
                {
                    if (!initialized)
                    {
                        LoadConfig().ConfigureAwait(false).GetAwaiter().GetResult(); // 加载配置
                    }
                }
            }
            return _config.KV;
        }
        /// <summary>
        /// 加载配置
        /// </summary>
        public async Task LoadConfig(bool reload = false)
        {
            var islocalcache = false;
            var localcachepath = System.IO.Path.Combine(AppContext.BaseDirectory, "localconfig.json"); // 容灾配置文件地址
            try
            {
                if (reload) _config.Version = 0; // 重载
                var client = _httpClientFactory.CreateClient(); // 创建http请求
                var serverUrl = _serviceLocator.GetConfigService(); // 配置请求地址
                var url = AssembleQueryConfigUrl(serverUrl);
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var configdto = _jsonHelper.DeserializeObject<HttpConfigResult>(content);
                // 请求成功并有更新配置
                if (configdto.ErrorCode == "000000" && configdto.Version > _config.Version)
                {
                    foreach (var kv in configdto.KV)
                    {
                        _config.KV.AddOrUpdate(kv.Key, kv.Value, (x, y) => kv.Value);
                    }
                    _config.AppName = configdto.AppName;
                    _config.Version = configdto.Version;
                    islocalcache = true;
                }
                _logger.LogInformation($"{_setting.AppId} loaded config {configdto}");
                // 本地缓存
                if (islocalcache)
                {
                    _logger.LogInformation($"配置中心配置信息写入本地文件:{localcachepath}");
                    string dir = System.IO.Path.GetDirectoryName(localcachepath);
                    if (!System.IO.Directory.Exists(dir))
                        System.IO.Directory.CreateDirectory(dir);
                    var json = _jsonHelper.SerializeObject(_config);
                    System.IO.File.WriteAllText(localcachepath, json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"config load error from appid {_setting.AppId}");
                if (System.IO.File.Exists(localcachepath))
                {
                    var json = System.IO.File.ReadAllText(localcachepath);
                    _config = _jsonHelper.DeserializeObject<BucketConfig>(json);
                }
                _logger.LogInformation($"config load error from appid {_setting.AppId},local disk cache recovery success.");
            }
            initialized = true; // 初始化成功
        }
        /// <summary>
        /// 配置中心请求参数配置
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string AssembleQueryConfigUrl(string url)
        {
            // 配置请求地址
            string appId = _setting.AppId;
            string secret = _setting.AppSercet;

            var uri = $"{url.TrimEnd('/')}/configs/{_setting.AppId}/{_setting.NamespaceName}";
            var query = $"version={_config.Version}";
            var sign = $"appId={appId}&appSecret={secret}&namespaceName={_setting.NamespaceName}";
            return $"{uri}?{query}&env={_setting.Env}&sign=" + SecureHelper.SHA256(sign);
        }
    }
}
