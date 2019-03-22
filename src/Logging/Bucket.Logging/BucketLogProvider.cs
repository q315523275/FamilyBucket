using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Bucket.Logging
{
    public class BucketLogProvider : ILoggerProvider
    {
        readonly ConcurrentDictionary<string, BucketLogLogger> _loggers = new ConcurrentDictionary<string, BucketLogLogger>();
        private readonly ILoggerDispatcher _loggerDispatcher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _applicationName;

        public BucketLogProvider(ILoggerDispatcher loggerDispatcher, IHttpContextAccessor httpContextAccessor, string applicationName)
        {
            _loggerDispatcher = loggerDispatcher;
            _httpContextAccessor = httpContextAccessor;
            _applicationName = applicationName;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return this._loggers.GetOrAdd(categoryName, p => { return new BucketLogLogger(_loggerDispatcher, _httpContextAccessor, _applicationName, categoryName); });
        }
        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
