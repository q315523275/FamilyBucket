using Bucket.EventBus.Common.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Logging
{
    public class BucketLogProvider : ILoggerProvider
    {
        /// <summary>
        /// event
        /// </summary>
        private readonly IEventBus _eventBus;
        /// <summary>
        /// The exception formatter Func.
        /// </summary>
        private Func<object, Exception, string> exceptionFormatter;
        readonly ConcurrentDictionary<string, BucketLogLogger> _loggers = new ConcurrentDictionary<string, BucketLogLogger>();
        public BucketLogProvider()
        {
        }
        public BucketLogProvider(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return this._loggers.GetOrAdd(categoryName, p => { return new BucketLogLogger(_eventBus); });
        }

        public void Dispose()
        {
          
        }
    }
}
