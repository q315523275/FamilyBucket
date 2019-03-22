using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Logging.Events
{
    public class LoggerHostedService : IHostedService
    {
        private readonly ILoggerDispatcher _loggerDispatcher;
        private readonly ILoggerTransport _loggerTransport;
        private Timer _timer;
        private bool _polling;

        public LoggerHostedService(ILoggerDispatcher loggerDispatcher, ILoggerTransport loggerTransport)
        {
            _loggerDispatcher = loggerDispatcher;
            _loggerTransport = loggerTransport;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var delay = 4 * 1000;
            _timer = new Timer(x =>
            {
                if (_polling || cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                _polling = true;
                Poll(cancellationToken);
                _polling = false;
            }, null, delay, delay);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void Poll(CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
                _loggerDispatcher.Flush(_loggerTransport, cancellationToken);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
