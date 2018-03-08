using Bucket.ErrorCode;
using Bucket.ErrorCodeStore.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Bucket.ErrorCodeStore
{
    public class DefaultErrorCodeStore : IErrorCodeStore
    {
        private readonly RemoteStoreRepository _storeRepository;
        private ILogger _logger;
        public DefaultErrorCodeStore(RemoteStoreRepository storeRepository, ILoggerFactory loggerFactory)
        {
            _storeRepository = storeRepository;
            _logger = loggerFactory.CreateLogger<DefaultErrorCodeStore>();
        }
        private void Initialize()
        {
            try
            {
                _storeRepository.GetStore();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Init Local ErrorCode failed reason: {ExceptionUtil.GetDetailMessage(ex)}.");
            }
            finally
            {
                // refresh
                _storeRepository.InitScheduleRefresh();
            }
        }
        public string StringGet(string code)
        {
            if (_storeRepository.GetStore().TryGetValue(code, out string value))
            {
                return value;
            }
            else
            {
                return string.Empty;
            }
        }

        public Task<string> StringGetAsync(string code)
        {
            if (_storeRepository.GetStore().TryGetValue(code, out string value))
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
