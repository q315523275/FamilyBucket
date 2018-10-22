using Microsoft.Extensions.Logging;

namespace Bucket.Config
{
    public class DefaultConfig : IConfig
    {
        private RemoteConfigRepository _configRepository;
        private ILogger _logger;
        public DefaultConfig(RemoteConfigRepository configRepository, ILoggerFactory loggerFactory)
        {
            _configRepository = configRepository;
            _logger = loggerFactory.CreateLogger<DefaultConfig>();
        }
        public string StringGet(string key)
        {
            if (_configRepository.GetConfig().TryGetValue(key, out string value))
            {
                return value;
            }
            else
            {
                return string.Empty;
            }
        }
        public string StringGet(string key, string defaultValue)
        {
            if (_configRepository.GetConfig().TryGetValue(key, out string value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
