using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
namespace Bucket.Config
{
    public class DefaultConfig : IConfig
    {
        private readonly IDataRepository _configRepository;
        private readonly ILogger<DefaultConfig> _logger;
        private bool _loaded = false;
        public DefaultConfig(IDataRepository configRepository, ILogger<DefaultConfig> logger)
        {
            _configRepository = configRepository;
            _logger = logger;
        }

        public string StringGet(string key)
        {
            if (!_loaded && _configRepository.Data.Count == 0)
                AsyncContext.Run(() => _configRepository.Get());

            _loaded = true;

            if (_configRepository.Data.TryGetValue(key, out string value))
                return value;
            else
                return string.Empty;
        }
        public string StringGet(string key, string defaultValue)
        {
            if (!_loaded && _configRepository.Data.Count == 0)
                AsyncContext.Run(() => _configRepository.Get());

            if (_configRepository.Data.TryGetValue(key, out string value))
                return value;
            else
                return defaultValue;
        }
    }
}
