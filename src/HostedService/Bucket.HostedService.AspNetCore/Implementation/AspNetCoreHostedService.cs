using Bucket.HostedService;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.HostedService.AspNetCore.Implementation
{
    public class AspNetCoreHostedService : IBucketAgentStartup
    {
        private readonly IEnumerable<IExecutionService> _services;

        public AspNetCoreHostedService(IEnumerable<IExecutionService> services)
        {
            _services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var service in _services)
                await service.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var service in _services)
                await service.StopAsync(cancellationToken);
        }
    }
}
