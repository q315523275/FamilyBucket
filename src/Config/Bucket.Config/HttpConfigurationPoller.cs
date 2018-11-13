using System;
using System.Threading;
using System.Threading.Tasks;
using Bucket.Core;
using Bucket.Redis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace Bucket.Config
{
    public class HttpConfigurationPoller : IHostedService, IDisposable
    {
        private readonly ConfigOptions _setting;
        private readonly RemoteConfigRepository _repository;
        private readonly RedisClient _redisClient;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILogger<HttpConfigurationPoller> _logger;
        private Timer _timer;
        private bool _polling;


        public HttpConfigurationPoller(ConfigOptions setting, 
            RemoteConfigRepository repository, 
            RedisClient redisClient, 
            IJsonHelper jsonHelper, 
            ILogger<HttpConfigurationPoller> logger)
        {
            _setting = setting;
            _repository = repository;
            _redisClient = redisClient;
            _jsonHelper = jsonHelper;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(HttpConfigurationPoller)} is starting.");

            var delay = _setting.RefreshInteval * 1000;
            _timer = new Timer(async x =>
            {
                if (_polling)
                {
                    return;
                }

                _polling = true;
                await Poll();
                _polling = false;
            }, null, delay, delay);

            AddListener();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(HttpConfigurationPoller)} is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async Task Poll()
        {
            _logger.LogInformation("Started polling");

            await _repository.LoadConfig();

            _logger.LogInformation("Finished polling");
        }

        /// <summary>
        /// 配置更新监听, redis方式
        /// </summary>
        private void AddListener()
        {
            if (_setting.RedisListener && !string.IsNullOrWhiteSpace(_setting.RedisConnectionString))
            {
                var sub = _redisClient.GetSubscriber(_setting.RedisConnectionString);
                sub.Subscribe("Bucket.Config.Listener", async (channel, message) =>
                {
                    var command = _jsonHelper.DeserializeObject<ConfigNetCommand>(message);
                    if (command != null && command.AppId == _setting.AppId && command.CommandType == EnumCommandType.ConfigUpdate)
                    {
                        // 更新
                        await _repository.LoadConfig();
                    }
                    if (command != null && command.AppId == _setting.AppId && command.CommandType == EnumCommandType.ConfigReload)
                    {
                        // 重载
                        await _repository.LoadConfig(true);
                    }
                });
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
            if (_setting.RedisListener && !string.IsNullOrWhiteSpace(_setting.RedisConnectionString))
            {
                var sub = _redisClient.GetSubscriber(_setting.RedisConnectionString);
                sub.Unsubscribe("Bucket.Config.Listener");
            }
        }
    }
}
