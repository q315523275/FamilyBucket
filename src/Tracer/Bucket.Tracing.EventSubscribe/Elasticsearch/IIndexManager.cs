using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracing.EventSubscribe.Elasticsearch
{
    public interface IIndexManager
    {
        IndexName CreateTracingIndex(DateTimeOffset? dateTimeOffset = null);

        IndexName CreateServiceIndex();
    }
}
