using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tracing.DataContract.Tracing;

namespace Tracing.Storage
{
    public interface ISpanStorage
    {
        Task StoreAsync(IEnumerable<Span> spans, CancellationToken cancellationToken = default(CancellationToken));
    }
}