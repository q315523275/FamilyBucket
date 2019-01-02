using Bucket.SkrTrace.Core.Transport;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.SkrTrace.Core.Abstractions
{
    public interface ISkrTraceCollect
    {
        Task CollectAsync(IEnumerable<TraceSegmentRequest> request, CancellationToken cancellationToken = default(CancellationToken));
    }
}
