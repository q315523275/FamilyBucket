using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.IO;
using System.Xml;

namespace Bucket.Logging.Log4Net
{
    public class Log4NetProvider : ILoggerProvider
    {
        private readonly string _log4NetConfigFile;
        private readonly ConcurrentDictionary<string, Log4NetLogger> _loggers = new ConcurrentDictionary<string, Log4NetLogger>();
        public Log4NetProvider(string log4NetConfigFile)
        {
            _log4NetConfigFile = log4NetConfigFile;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, new Log4NetLogger(categoryName, Parselog4NetConfigFile(_log4NetConfigFile)));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }

        private static XmlElement Parselog4NetConfigFile(string filename)
        {
            XmlDocument log4netConfig = new XmlDocument();
            var stream = File.OpenRead(filename);
            log4netConfig.Load(stream);
            stream.Close();
            return log4netConfig["log4net"];
        }
    }
}
