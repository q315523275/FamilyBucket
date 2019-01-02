using Bucket.SkrTrace.Core.Transport;
using System;

namespace Bucket.SkrTrace.Core.Abstractions
{
    public interface ITraceSegmentRef : IEquatable<ITraceSegmentRef>
    {
        string EntryOperationName { get; }
        string EntryApplicationCode { get; }
        TraceSegmentReferenceRequest Transform();
    }
}
