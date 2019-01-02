using Bucket.SkrTrace.Core.Abstractions;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.SkrTrace.Core
{
    public class InstrumentationHostedService : IHostedService
    {
        private readonly ISkrTraceAgentStartup _startup;

        public InstrumentationHostedService(ISkrTraceAgentStartup startup)
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
