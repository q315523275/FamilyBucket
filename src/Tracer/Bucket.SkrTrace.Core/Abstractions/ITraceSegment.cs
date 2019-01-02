using Bucket.SkrTrace.Core.OpenTracing;
using Bucket.SkrTrace.Core.Transport;
using System.Collections.Generic;

namespace Bucket.SkrTrace.Core.Abstractions
{
    public interface ITraceSegment
    {
        void Archive(AbstractTracingSpan finishedSpan);
        string ApplicationCode { get; }
        string TraceSegmentId { set; get; }
        string Identity { get; set; }
        ITraceSegment Finish();
        void Ref(ITraceSegmentRef refSegment);
        IEnumerable<ITraceSegmentRef> Refs { get; }
        TraceSegmentRequest Transform();
    }
}
