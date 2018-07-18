using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Bucket.Config.Util;
using Bucket.LoadBalancer;
using Bucket.Config.Util.Http;
using Bucket.Redis;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Bucket.Core;
namespace Bucket.Config
{
    /// <summary>
    /// 远程配置仓储
    /// </summary>
    public class RemoteConfigRepository: IDisposable
    {
        private BucketConfig _config;
        private ConfigSetting _setting;
        private CancellationTokenSource _cancellationTokenSource;
        private RedisClient _redisClient;
        private ILoadBalancerHouse _loadBalancerHouse;
        private IJsonHelper _jsonHelper;
        private ILogger _logger;
        private static readonly object _lock = new object();
        public RemoteConfigRepository(ConfigSetting setting, 
            RedisClient redisClient, 
            ILoadBalancerHouse loadBalancerHouse,
            ILoggerFactory loggerFactory,
            IJsonHelper jsonHelper)
        {
            _logger = loggerFactory.CreateLogger<RemoteConfigRepository>();
            _config = new BucketConfig();
            _setting = setting;
            _redisClient = redisClient;
            _loadBalancerHouse = loadBalancerHouse;
            _jsonHelper = jsonHelper;
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
                var url = GetQueryConfigUrl();
                _logger.LogInformation($"{_setting.AppId} loading config from  {url}");
                var response = HttpUtil.Get<HttpConfigResult>(new HttpRequest(url), _jsonHelper);
                _logger.LogInformation($"{_setting.AppId} config server responds with {response.StatusCode} HTTP status code.");
                // request error
                if (response.Body.ErrorCode != "000000")
                    _logger.LogInformation($"{_setting.AppId} config request error:" + response.Body.Message);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.Body.Version > _config.Version)
                    {
                        if (_config.KV == null)
                            _config.KV = new ConcurrentDictionary<string, string>();
                        foreach (var kv in response.Body.KV)
                        {
                            _config.KV.AddOrUpdate(kv.Key, kv.Value, (x, y) => kv.Value);
                        }
                        _config.AppName = response.Body.AppName;
                        _config.Version = response.Body.Version;
                        islocalcache = true;
                    }
                    _logger.LogInformation($"{_setting.AppId} loaded config {response.Body}");
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
        private string GetQueryConfigUrl()
        {
            string url = string.Empty;
            if (_setting.UseServiceDiscovery && _loadBalancerHouse != null)
            {
                var _load = _loadBalancerHouse.Get(_setting.ServiceName, "RoundRobin").Result;
                var HostAndPort = _load.Lease().Result;
                url = $"{HostAndPort.ToUri()}";
            }
            else
            {
                url = _setting.ServerUrl;
            }
            string appId = _setting.AppId;
            string secret = _setting.AppSercet;

            var uri = $"{url.TrimEnd('/')}/configs/{_setting.AppId}/{_setting.NamespaceName}";
            var query = $"version={_config.Version}";
            var sign = $"appId={appId}&appSecret={secret}&namespaceName={_setting.NamespaceName}";
            return $"{uri}?{query}&sign=" + SecureHelper.SHA256(sign);
        }
        public void InitScheduleRefresh()
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
