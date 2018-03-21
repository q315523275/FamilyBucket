using Bucket.EventBus.Common.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Logging
{
    public class BucketLogProvider : ILoggerProvider
    {
        private readonly string _projectName;
        private Func<object, Exception, string> exceptionFormatter;
        readonly ConcurrentDictionary<string, BucketLogLogger> _loggers = new ConcurrentDictionary<string, BucketLogLogger>();
        private readonly IEventBus _eventBus;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BucketLogProvider(IEventBus eventBus, IHttpContextAccessor httpContextAccessor, string projectName)
        {
            _eventBus = eventBus;
            _projectName = projectName;
            _httpContextAccessor = httpContextAccessor;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return this._loggers.GetOrAdd(categoryName, p => { return new BucketLogLogger(_eventBus, _httpContextAccessor, _projectName); });
        }

        public void Dispose()
        {
          
        }
    }
}
