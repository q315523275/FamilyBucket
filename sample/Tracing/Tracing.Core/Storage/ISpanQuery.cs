using System.Collections.Generic;
using System.Threading.Tasks;
using Tracing.DataContract.Tracing;
using Tracing.Storage.Query;

namespace Tracing.Storage
{
    public interface ISpanQuery
    {
        Task<Span> GetSpan(string spanId);

        Task<Trace> GetTrace(string traceId);

        Task<IEnumerable<Trace>> GetTraces(TraceQuery traceQuery);

        Task<IEnumerable<Span>> GetSpanDependencies(DependencyQuery dependencyQuery);

        Task<IEnumerable<TraceHistogram>> GetTraceHistogram(TraceQuery traceQuery);
    }
}