using Bucket.Tracing.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Tracing.EventSubscribe.Elasticsearch
{
    public interface ISpanStorage
    {
        Task StoreAsync(IEnumerable<Span> spans, CancellationToken cancellationToken = default(CancellationToken));
    }
}
