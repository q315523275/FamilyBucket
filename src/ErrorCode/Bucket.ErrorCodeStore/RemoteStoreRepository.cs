using Bucket.Core;
using Bucket.ErrorCode;
using Bucket.ErrorCodeStore.Model;
using Bucket.ErrorCodeStore.Util;
using Bucket.ErrorCodeStore.Util.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.ErrorCodeStore
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
        private ManualResetEventSlim _eventSlim;
        private IJsonHelper _jsonHelper;
        public RemoteStoreRepository(ErrorCodeSetting errorCodeConfiguration, ILoggerFactory loggerFactory, IJsonHelper jsonHelper)
        {
            _logger = loggerFactory.CreateLogger<RemoteStoreRepository>();
            _config = new ErrorCodeConfig();
            _errorCodeConfiguration = errorCodeConfiguration;
            _jsonHelper = jsonHelper;
        }
        protected void Sync()
        {
            lock (this)
            {
                try
                {
                    LoadErrorCodeStore().GetAwaiter();
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
            _logger.LogInformation($"erroCode schedule refresh with interval: {_errorCodeConfiguration.RefreshInteval} s");
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                Task.Factory.StartNew(() => { Thread.Sleep(_errorCodeConfiguration.RefreshInteval * 1000); _eventSlim.Set(); });
                _logger.LogInformation($"refresh errorcode for ding");
                Sync();
                try
                {
                    _eventSlim.Wait(_cancellationTokenSource.Token);
                    _eventSlim.Reset();
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError($"load errorcode from ding error !\r\n exception: {ExceptionUtil.GetDetailMessage(ex)}");
                }
            }
        }
        public ConcurrentDictionary<string, string> GetStore()
        {
            if (_config.KV == null)
            {
                _config.KV = new ConcurrentDictionary<string, string>();
                Sync();
            }
            return _config.KV;
        }
        private async Task LoadErrorCodeStore()
        {
            var islocalcache = false;
            var localcachepath = System.IO.Path.Combine(AppContext.BaseDirectory, "localerrorcode.json");
            try
            {
                var url = AssembleQueryConfigUrl();
                _logger.LogInformation($"loading errorcode from  {url}");
                var response = await HttpUtil.Get<ApiInfo>(new HttpRequest(url), _jsonHelper);
                _logger.LogInformation($"errorcode server responds with {response.StatusCode} HTTP status code.");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.Body != null && response.Body.Value != null && response.Body.Value.Count > 0)
                    {
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
                _logger.LogError($"errorCode load error from ding: {ExceptionUtil.GetDetailMessage(ex)}");
                if (System.IO.File.Exists(localcachepath))
                {
                    var json = System.IO.File.ReadAllText(localcachepath);
                    _config = _jsonHelper.DeserializeObject<ErrorCodeConfig>(json);
                }
                _logger.LogInformation($"errorCode load error from ding,local disk cache recovery success.");
            }
        }
        private string AssembleQueryConfigUrl()
        {
            string url = _errorCodeConfiguration.ServerUrl.TrimEnd('/');

            var uri = $"{url}/ErrorCode/GetList";
            var query = $"source=PZGO";
            return $"{uri}?{query}";
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
