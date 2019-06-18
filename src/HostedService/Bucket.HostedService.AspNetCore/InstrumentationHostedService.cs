using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.HostedService.AspNetCore
{
    public class InstrumentationHostedService : IHostedService
    {
        private readonly IBucketAgentStartup _startup;

        public InstrumentationHostedService(IBucketAgentStartup startup)
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
