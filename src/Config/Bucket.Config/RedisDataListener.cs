using System;
using Bucket.Core;
using Bucket.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bucket.Config
{
    public class RedisDataListener : IDataListener, IDisposable
    {
        private readonly ConfigOptions _setting;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDataRepository _dataRepository;
        private readonly ILogger<RedisDataListener> _logger;
        private readonly IJsonHelper _jsonHelper;
        private readonly string _RedisListenerKey = "Bucket.Config.Listener";
        private RedisClient _redisClient;
        public RedisDataListener(ConfigOptions setting, IServiceProvider serviceProvider, IDataRepository dataRepository, ILogger<RedisDataListener> logger, IJsonHelper jsonHelper)
        {
            _setting = setting;
            _serviceProvider = serviceProvider;
            _dataRepository = dataRepository;
            _logger = logger;
            _jsonHelper = jsonHelper;
        }

        public void AddListener()
        {
            if (_setting.RedisListener && !string.IsNullOrWhiteSpace(_setting.RedisConnectionString))
            {
                _redisClient = _serviceProvider.GetRequiredService<RedisClient>();
                var subscriber = _redisClient.GetSubscriber(_setting.RedisConnectionString);
                subscriber.Subscribe(_RedisListenerKey, async (channel, message) =>
                {
                    var command = _jsonHelper.DeserializeObject<ConfigNetCommand>(message);
                    if (command != null && command.AppId == _setting.AppId && command.NamespaceName == _setting.NamespaceName && command.CommandType == EnumCommandType.ConfigUpdate)
                    {
                        // 更新
                        await _dataRepository.Get();
                    }
                    if (command != null && command.AppId == _setting.AppId && command.NamespaceName == _setting.NamespaceName && command.CommandType == EnumCommandType.ConfigReload)
                    {
                        // 重载
                        await _dataRepository.Get(true);
                    }
                });
            }
        }

        public void Dispose()
        {
            if (_redisClient != null)
            {
                var subscriber = _redisClient.GetSubscriber(_setting.RedisConnectionString);
                subscriber.Unsubscribe(_RedisListenerKey);
            }
        }
    }
}
