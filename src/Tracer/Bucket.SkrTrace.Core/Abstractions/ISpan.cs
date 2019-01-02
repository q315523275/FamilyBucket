using Bucket.SkrTrace.Core.OpenTracing;
using Bucket.SkrTrace.Core.Transport;
using System;

namespace Bucket.SkrTrace.Core.Abstractions
{
    public interface ISpan
    {
        string OperationName { get; set; }
        string SpanId { get; }
        string ParentSpanId { get; set; }
        TagCollection Tags { get; }
        LogCollection Logs { get; }
        bool IsEntry { get; }
        bool IsExit { get; }
        ISpan SetComponent(SpanComponent spanComponent);
        ISpan ErrorOccurred();
        ISpan Start();
        ISpan Start(DateTimeOffset timestamp);
        ISpan SetLayer(SpanLayer layer);
        bool Finish(ITraceSegment owner);
        void Ref(ITraceSegmentRef traceSegmentRef);
        SpanRequest Transform();
    }
}
