using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Listener.Abstractions
{
    public interface IListenerAgentStartup
    {
        Task StartAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task StopAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
