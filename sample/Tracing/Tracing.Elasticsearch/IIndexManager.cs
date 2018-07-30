using System;
using System.Collections.Generic;
using System.Text;
using Nest;

namespace Tracing.Elasticsearch
{
    public interface IIndexManager
    {
        IndexName CreateTracingIndex(DateTimeOffset? dateTimeOffset = null);

        IndexName CreateServiceIndex();
    }
}