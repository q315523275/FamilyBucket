using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Bucket.Config.Util;
using Bucket.LoadBalancer;
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
    public class RemoteConfigRepository: IDisposable
    {
        private BucketConfig _config;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ConfigSetting _setting;
        private readonly RedisClient _redisClient;
        private ConfigServiceLocator _serviceLocator;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private static readonly object _lock = new object();
        public RemoteConfigRepository(ConfigSetting setting, 
            RedisClient redisClient,
            ConfigServiceLocator configServiceLocator,
            ILoggerFactory loggerFactory,
            IJsonHelper jsonHelper,
            IHttpClientFactory httpClientFactory)
        {
            _logger = loggerFactory.CreateLogger<RemoteConfigRepository>();
            _config = new BucketConfig();

            _setting = setting;
            _redisClient = redisClient;

            _serviceLocator = configServiceLocator;

            _jsonHelper = jsonHelper;
            _httpClientFactory = httpClientFactory;
        }
        public ConcurrentDictionary<string, string> GetConfig()
        {
            if(_config.KV == null)
            {
                lock (_lock)
                {
                    if (_config.KV == null)
                    {
                        LoadConfig();
                        InitScheduleRefresh();
                        AddChangeListener();
                    }
                }
            }
            return _config.KV;
        }
        private void LoadConfig()
        {
            var islocalcache = false;
            var localcachepath = System.IO.Path.Combine(AppContext.BaseDirectory, "localconfig.json");
            try
            {
                var client = _httpClientFactory.CreateClient();
                var serverUrl = _serviceLocator.GetConfigService(); // 配置请求地址
                var url = AssembleQueryConfigUrl(serverUrl);
                // 死锁问题
                var response = client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)).ConfigureAwait(false).GetAwaiter().GetResult();
                if(!response.IsSuccessStatusCode)
                    _logger.LogError($"{_setting.AppId} config request error status {response.StatusCode}");
                if (response.IsSuccessStatusCode)
                {
                    var configdto = _jsonHelper.DeserializeObject<HttpConfigResult>(response.Content.ReadAsStringAsync().Result);
                    if (configdto.Version > _config.Version)
                    {
                        if (_config.KV == null)
                            _config.KV = new ConcurrentDictionary<string, string>();
                        foreach (var kv in configdto.KV)
                        {
                            _config.KV.AddOrUpdate(kv.Key, kv.Value, (x, y) => kv.Value);
                        }
                        _config.AppName = configdto.AppName;
                        _config.Version = configdto.Version;
                        islocalcache = true;
                    }
                    _logger.LogInformation($"{_setting.AppId} loaded config {configdto}");
                }
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
                _logger.LogError($"config load error from appid {_setting.AppId}", ex);
                if (System.IO.File.Exists(localcachepath))
                {
                    var json = System.IO.File.ReadAllText(localcachepath);
                    _config = _jsonHelper.DeserializeObject<BucketConfig>(json);
                }
                _logger.LogInformation($"config load error from appid {_setting.AppId},local disk cache recovery success.");
            }
        }
        private string AssembleQueryConfigUrl(string url)
        {
            // 配置请求地址
            string appId = _setting.AppId;
            string secret = _setting.AppSercet;

            var uri = $"{url.TrimEnd('/')}/configs/{_setting.AppId}/{_setting.NamespaceName}";
            var query = $"version={_config.Version}";
            var sign = $"appId={appId}&appSecret={secret}&namespaceName={_setting.NamespaceName}";
            return $"{uri}?{query}&sign=" + SecureHelper.SHA256(sign);
        }
        private void InitScheduleRefresh()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var _processQueueTask = Task.Factory.StartNew(ScheduleRefresh, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        private void ScheduleRefresh()
        {
            _logger.LogInformation($"schedule refresh with interval: {_setting.RefreshInteval} s");
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                Thread.Sleep(_setting.RefreshInteval * 1000);
                _logger.LogInformation($"refresh config for appid: {_setting.AppId}");
                LoadConfig();
            }
        }
        private void AddChangeListener()
        {
            if (_setting.RedisListener && !string.IsNullOrWhiteSpace(_setting.RedisConnectionString))
            {
                ISubscriber sub = _redisClient.GetSubscriber(_setting.RedisConnectionString);
                sub.SubscribeAsync("Bucket_Config_ChangeListener", (channel, message) =>
                {
                    var command = _jsonHelper.DeserializeObject<ConfigNetCommand>(message);
                    if(command != null && command.AppId == _setting.AppId && command.CommandType == EnumCommandType.ConfigUpdate)
                    {
                        // 更新
                        LoadConfig();
                    }
                    if (command != null && command.AppId == _setting.AppId && command.CommandType == EnumCommandType.ConfigReload)
                    {
                        // 重载
                        _config.Version = 0;
                        LoadConfig();
                    }
                });
            }
        }
        public void Dispose()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
            if(_setting.RedisListener && !string.IsNullOrWhiteSpace(_setting.RedisConnectionString))
            {
                ISubscriber sub = _redisClient.GetSubscriber(_setting.RedisConnectionString);
                sub.UnsubscribeAsync("Bucket_Config_ChangeListener");
            }
        }
    }
}
