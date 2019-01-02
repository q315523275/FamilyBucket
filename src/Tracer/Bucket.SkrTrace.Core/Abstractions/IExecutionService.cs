using System.Threading;
using System.Threading.Tasks;
namespace Bucket.SkrTrace.Core.Abstractions
{
    public interface IExecutionService
    {
        Task StartAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task StopAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
