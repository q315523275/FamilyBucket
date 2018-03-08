using Bucket.ConfigCenter.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.ConfigCenter
{
    public class DefaultConfig : IConfigCenter
    {
        private RemoteConfigRepository _configRepository;
        private ILogger _logger;
        public DefaultConfig(RemoteConfigRepository configRepository, ILoggerFactory loggerFactory)
        {
            _configRepository = configRepository;
            _logger = loggerFactory.CreateLogger<DefaultConfig>();
            Initialize();
        }
        private void Initialize()
        {
            try
            {
                _configRepository.GetConfig();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Init Local Config failed reason: {ExceptionUtil.GetDetailMessage(ex)}.");
            }
            finally
            {
                // refresh
                _configRepository.InitScheduleRefresh();
                //register the change listener no matter config repository is working or not
                //so that whenever config repository is recovered, config could get changed
                _configRepository.AddChangeListener();
            }
        }
        public string Get(string key)
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

        public Task<string> GetAsync(string key)
        {
            if (_configRepository.GetConfig().TryGetValue(key, out string value))
            {
                return Task.FromResult(value);
            }
            else
            {
                return Task.FromResult(string.Empty);
            }
        }
    }
}
