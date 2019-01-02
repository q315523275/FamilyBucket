using Bucket.Listener.Abstractions;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Listener
{
    public class InstrumentationHostedService : IHostedService
    {
        private readonly IListenerAgentStartup _startup;

        public InstrumentationHostedService(IListenerAgentStartup startup)
        {
            _startup = startup;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _startup.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _startup.StopAsync(cancellationToken);
        }
    }
}
