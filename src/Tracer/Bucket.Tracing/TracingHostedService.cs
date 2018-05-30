using Bucket.Tracing.Diagnostics;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Tracing
{
    public class TracingHostedService : IHostedService
    {
        public TracingHostedService(TracingDiagnosticListenerObserver diagnosticObserver)
        {
            DiagnosticListener.AllListeners.Subscribe(diagnosticObserver);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
