using Bucket.Core;
using Bucket.ErrorCode;
using Bucket.ErrorCode.Model;
using Bucket.ErrorCode.Util;
using Bucket.ErrorCode.Util.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.ErrorCode
{
    /// <summary>
    /// 远程仓库
    /// </summary>
    public class RemoteStoreRepository
    {
        private ErrorCodeConfig _config;
        private ErrorCodeSetting _errorCodeConfiguration;
        private CancellationTokenSource _cancellationTokenSource;
        private ILogger _logger;
        private IJsonHelper _jsonHelper;
        private static readonly object _lock = new object();
        public RemoteStoreRepository(IOptions<ErrorCodeSetting> errorCodeConfiguration, ILoggerFactory loggerFactory, IJsonHelper jsonHelper)
        {
            _logger = loggerFactory.CreateLogger<RemoteStoreRepository>();
            _config = new ErrorCodeConfig();
            _errorCodeConfiguration = errorCodeConfiguration.Value;
            _jsonHelper = jsonHelper;
        }
        public ConcurrentDictionary<string, string> GetStore()
        {
            if (_config.KV == null)
            {
                lock (_lock)
                {
                    if (_config.KV == null)
                    {
                        LoadErrorCodeStore();
                        InitScheduleRefresh();
                    }
                }
            }
            return _config.KV;
        }
        private void LoadErrorCodeStore()
        {
            var islocalcache = false;
            var localcachepath = System.IO.Path.Combine(AppContext.BaseDirectory, "localerrorcode.json");
            try
            {
                var url = GetQueryConfigUrl();
                _logger.LogInformation($"loading errorcode from  {url}");
                var response = HttpUtil.Get<ApiInfo>(new HttpRequest(url), _jsonHelper);
                _logger.LogInformation($"errorcode server responds with {response.StatusCode} HTTP status code.");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.Body != null && response.Body.Value != null && response.Body.Value.Count > 0)
                    {
                        if (_config.KV == null)
                            _config.KV = new ConcurrentDictionary<string, string>();
                        foreach (var kv in response.Body.Value)
                        {
                            _config.KV.AddOrUpdate(kv.ErrorCode, kv.ErrorMessage, (x, y) => kv.ErrorMessage);
                        }
                        islocalcache = true;
                    }
                    _logger.LogInformation($"Loaded errorcode {response.Body}");
                }
                if (islocalcache)
                {
                    _logger.LogInformation($"错误码中心配置信息写入本地文件:{localcachepath}");
                    string dir = System.IO.Path.GetDirectoryName(localcachepath);
                    if (!System.IO.Directory.Exists(dir))
                        System.IO.Directory.CreateDirectory(dir);
                    var json = _jsonHelper.SerializeObject(_config);
                    System.IO.File.WriteAllText(localcachepath, json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"errorCode load error from ding:", ex);
                if (System.IO.File.Exists(localcachepath))
                {
                    var json = System.IO.File.ReadAllText(localcachepath);
                    _config = _jsonHelper.DeserializeObject<ErrorCodeConfig>(json);
                }
                _logger.LogInformation($"errorCode load error from ding,local disk cache recovery success.");
            }
        }
        private string GetQueryConfigUrl()
        {
            string url = _errorCodeConfiguration.ServerUrl.TrimEnd('/');

            var uri = $"{url}/ErrorCode/GetList";
            var query = $"source=PZGO";
            return $"{uri}?{query}";
        }
        private void InitScheduleRefresh()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var _processQueueTask = Task.Factory.StartNew(ScheduleRefresh, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        private void ScheduleRefresh()
        {
            _logger.LogInformation($"erroCode schedule refresh with interval: {_errorCodeConfiguration.RefreshInteval} s");
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                Thread.Sleep(_errorCodeConfiguration.RefreshInteval * 1000);
                _logger.LogInformation($"refresh errorcode for ding");
                LoadErrorCodeStore();
            }
        }
        public void Dispose()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }
    }
}
