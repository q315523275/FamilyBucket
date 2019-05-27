using Bucket.Config.Abstractions;
using Bucket.Config.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Bucket.Config
{
    public class DefaultConfig : IConfig
    {
        private readonly IDataRepository _configRepository;
        private readonly ILogger<DefaultConfig> _logger;
        private readonly ThreadSafe.Boolean _loaded;
        public DefaultConfig(IDataRepository configRepository, ILogger<DefaultConfig> logger)
        {
            _configRepository = configRepository;
            _logger = logger;
            _loaded = new ThreadSafe.Boolean(false);
        }
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string StringGet(string key)
        {
            if (!_loaded.ReadFullFence())
                _configRepository.Get().ConfigureAwait(false).GetAwaiter().GetResult();

            _loaded.WriteFullFence(true);

            if (_configRepository.Data.TryGetValue(key, out string value))
                return value;
            else
                return string.Empty;
        }
        public async Task<string> StringGetAsync(string key)
        {
            if (!_loaded.ReadFullFence())
                await _configRepository.Get();

            _loaded.WriteFullFence(true);

            if (_configRepository.Data.TryGetValue(key, out string value))
                return value;
            else
                return string.Empty;
        }
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string StringGet(string key, string defaultValue)
        {
            if (!_loaded.ReadFullFence())
                _configRepository.Get().ConfigureAwait(false).GetAwaiter().GetResult();

            _loaded.WriteFullFence(true);

            if (_configRepository.Data.TryGetValue(key, out string value))
                return value;
            else
                return defaultValue;
        }
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TResult StringGet<TResult>(string key)
        {
            if (!_loaded.ReadFullFence())
                _configRepository.Get().ConfigureAwait(false).GetAwaiter().GetResult();

            _loaded.WriteFullFence(true);

            if (_configRepository.Data.TryGetValue(key, out string value))
                return (TResult)Convert.ChangeType(value, typeof(TResult));
            else
                return default;
        }
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<TResult> StringGetAsync<TResult>(string key)
        {
            if (!_loaded.ReadFullFence())
                await _configRepository.Get();

            _loaded.WriteFullFence(true);

            if (_configRepository.Data.TryGetValue(key, out string value))
                return (TResult)Convert.ChangeType(value, typeof(TResult));
            else
                return default;
        }
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public TResult StringGet<TResult>(string key, TResult defaultValue)
        {
            if (!_loaded.ReadFullFence())
                _configRepository.Get().ConfigureAwait(false).GetAwaiter().GetResult();

            _loaded.WriteFullFence(true);

            if (_configRepository.Data.TryGetValue(key, out string value))
                return (TResult)Convert.ChangeType(value, typeof(TResult));
            else
                return defaultValue;
        }
    }
}
