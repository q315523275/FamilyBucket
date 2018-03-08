using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Bucket.ConfigCenter.Util;
using Bucket.LoadBalancer;
using Bucket.ConfigCenter.Util.Http;
using Bucket.Redis;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Bucket.Core;

namespace Bucket.ConfigCenter
{
    /// <summary>
    /// 远程配置仓储
    /// </summary>
    public class RemoteConfigRepository
    {
        private BucketConfig _config;
        private ConfigCenterConfiguration _configCenterConfiguration;
        private CancellationTokenSource _cancellationTokenSource;
        private RedisClient _redisClient;
        private ILoadBalancerHouse _loadBalancerHouse;
        private ILogger _logger;
        private ManualResetEventSlim _eventSlim;
        private IJsonHelper _jsonHelper;
        public RemoteConfigRepository(ConfigCenterConfiguration config, 
            RedisClient redisClient, 
            ILoadBalancerHouse loadBalancerHouse,
            ILoggerFactory loggerFactory,
            IJsonHelper jsonHelper)
        {
            _logger = loggerFactory.CreateLogger<RemoteConfigRepository>();
            _config = new BucketConfig();
            _configCenterConfiguration = config;
            _redisClient = redisClient;
            _loadBalancerHouse = loadBalancerHouse;
            _jsonHelper = jsonHelper;
        }
        protected void Sync()
        {
            lock (this)
            {
                try
                {
                    LoadBucketConfig().GetAwaiter();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// 定时刷新
        /// </summary>
        public void InitScheduleRefresh()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _eventSlim = new ManualResetEventSlim(false, spinCount: 1);
            var _processQueueTask = Task.Factory.StartNew(ScheduleRefresh, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        private void ScheduleRefresh()
        {
            _logger.LogInformation($"Schedule refresh with interval: {_configCenterConfiguration.RefreshInteval} s");
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                Task.Factory.StartNew(() => { Thread.Sleep(_configCenterConfiguration.RefreshInteval * 1000); _eventSlim.Set(); });
                _logger.LogInformation($"refresh config for appid: {_configCenterConfiguration.AppId}");
                Sync();
                try
                {
                    _eventSlim.Wait(_cancellationTokenSource.Token);
                    _eventSlim.Reset();
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError($"load config from appid: {_configCenterConfiguration.AppId} error !\r\n exception: {ExceptionUtil.GetDetailMessage(ex)}");
                }
            }
        }
        public ConcurrentDictionary<string, string> GetConfig()
        {
            if(_config.KV == null)
            {
                _config.KV = new ConcurrentDictionary<string, string>();
                Sync();
            }
            return _config.KV;
        }
        /// <summary>
        /// 变更监听
        /// </summary>
        public void AddChangeListener()
        {
            if (_configCenterConfiguration.RedisListener && !string.IsNullOrWhiteSpace(_configCenterConfiguration.RedisConnectionString))
            {
                ISubscriber sub = _redisClient.GetSubscriber(_configCenterConfiguration.RedisConnectionString);
                sub.SubscribeAsync("Bucket_ConfigCenter_Event", (channel, message) =>
                {
                    if (message == _configCenterConfiguration.AppId) Sync();
                });
            }
        }
        private async Task LoadBucketConfig()
        {
            var islocalcache = false;
            var localcachepath = AppContext.BaseDirectory + "\\temp\\localconfig.json";
            try
            {
                var url = AssembleQueryConfigUrl();
                _logger.LogInformation($"loading config from  {url}");
                var response = await HttpUtil.Get<BucketConfig>(new HttpRequest(url), _jsonHelper);
                _logger.LogInformation($"config  server responds with {response.StatusCode} HTTP status code.");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                   if(response.Body.Version > _config.Version)
                    {
                        foreach (var kv in response.Body.KV)
                        {
                            _config.KV.AddOrUpdate(kv.Key, kv.Value, (x, y) => kv.Value);
                        }
                        _config.Version = response.Body.Version;
                        islocalcache = true;
                    }
                    _logger.LogInformation($"Loaded config {response.Body}");
                }
                if (islocalcache)
                {
                    string dir = System.IO.Path.GetDirectoryName(localcachepath);
                    if (!System.IO.Directory.Exists(dir))
                        System.IO.Directory.CreateDirectory(dir);
                    var json = _jsonHelper.SerializeObject(_config);
                    System.IO.File.WriteAllText(localcachepath, json);
                }
            }
            //catch (RemoteStatusCodeException ex)
            //{
            //    if (ex.StatusCode == HttpStatusCode.NotFound)
            //    {
                    
            //    }
            //}
            catch (Exception ex)
            {
                _logger.LogError($"config load error from appid {_configCenterConfiguration.AppId}: {ExceptionUtil.GetDetailMessage(ex)}");
                if (System.IO.File.Exists(localcachepath))
                {
                    var json = System.IO.File.ReadAllText(localcachepath);
                    _config = _jsonHelper.DeserializeObject<BucketConfig>(json);
                }
                _logger.LogInformation($"config load error from appid {_configCenterConfiguration.AppId},local disk cache recovery success.");
            }
        }
        private string AssembleQueryConfigUrl()
        {
            string url = string.Empty;
            if (_configCenterConfiguration.UseServiceDiscovery && _loadBalancerHouse != null)
            {
                var _load = _loadBalancerHouse.Get(_configCenterConfiguration.ServiceName, "RoundRobin").GetAwaiter().GetResult();
                var HostAndPort = _load.Lease().GetAwaiter().GetResult();
                url = $"{HostAndPort.ToUri()}";
            }
            else
            {
                url = _configCenterConfiguration.ServerUrl;
            }
            string appId = _configCenterConfiguration.AppId;
            string secret = _configCenterConfiguration.AppSercet;

            var uri = $"{url}services/config";
            var query = $"appId={appId}&version={_config.Version}";
            var sign = $"appId={appId}&appSecret={secret}";
            return $"{uri}?{query}&sign="+ SecureHelper.SHA256(sign);
        }
        public void Dispose()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
            if(_configCenterConfiguration.RedisListener && !string.IsNullOrWhiteSpace(_configCenterConfiguration.RedisConnectionString))
            {
                ISubscriber sub = _redisClient.GetSubscriber(_configCenterConfiguration.RedisConnectionString);
                sub.UnsubscribeAsync("Bucket_ConfigCenter_Event");
            }
        }
    }
}
