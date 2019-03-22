using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Bucket.Logging.Nlog
{
    public class NLogProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, NLogLogger> _loggers = new ConcurrentDictionary<string, NLogLogger>();

        public NLogProvider()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, new NLogLogger(categoryName));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
