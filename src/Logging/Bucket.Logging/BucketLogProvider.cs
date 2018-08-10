using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace Bucket.Logging
{
    public class BucketLogProvider : ILoggerProvider
    {
        private readonly string _projectName;
        private Func<object, Exception, string> exceptionFormatter;
        readonly ConcurrentDictionary<string, BucketLogLogger> _loggers = new ConcurrentDictionary<string, BucketLogLogger>();
        private readonly ILogStore _logStore;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BucketLogProvider(ILogStore logStore, IHttpContextAccessor httpContextAccessor, string projectName)
        {
            _logStore = logStore;
            _projectName = projectName;
            _httpContextAccessor = httpContextAccessor;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return this._loggers.GetOrAdd(categoryName, p => { return new BucketLogLogger(_logStore, _httpContextAccessor, _projectName, categoryName); });
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
