using System.Threading;
using System.Threading.Tasks;

namespace Bucket.SkrTrace.Core.Abstractions
{
    public interface ISkrTraceAgentStartup
    {
        Task StartAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task StopAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
