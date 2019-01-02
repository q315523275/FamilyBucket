using System.Threading;
using System.Threading.Tasks;

namespace Bucket.HostedService
{
    public interface IBucketAgentStartup
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
