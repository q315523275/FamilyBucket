using Bucket.Tracing.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Tracing
{
    public class TracingHostedService : IHostedService
    {
        private readonly TracingDiagnosticListenerObserver _diagnosticObserver;
        private readonly ILogger _logger;
        public TracingHostedService(TracingDiagnosticListenerObserver diagnosticObserver, ILoggerFactory loggerFactory)
        {
            _diagnosticObserver = diagnosticObserver;
            _logger = loggerFactory.CreateLogger<TracingHostedService>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                DiagnosticListener.AllListeners.Subscribe(_diagnosticObserver);
            }
            catch (Exception e)
            {
                _logger.LogError("TracingHostedService Start Fail.", e);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
