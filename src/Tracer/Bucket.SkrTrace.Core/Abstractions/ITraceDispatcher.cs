using Bucket.SkrTrace.Core.Transport;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.SkrTrace.Core.Abstractions
{
    public interface ITraceDispatcher
    {
        bool Dispatch(TraceSegmentRequest segment);
        Task Flush(CancellationToken token = default(CancellationToken));
        void Close();
    }
}
