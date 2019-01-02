using System;
using System.Collections.Generic;

namespace Bucket.SkrTrace.Core.Transport
{
    public class TraceSegmentRequest
    {
        public TraceSegmentObjectRequest Segment { get; set; }
    }
    public class TraceSegmentObjectRequest
    {
        public string SegmentId { get; set; }
        public string ApplicationCode { get; set; }
        public string Identity { get; set; }
        public IList<SpanRequest> Spans { get; set; } = new List<SpanRequest>();
    }

    public class SpanRequest
    {
        public string SpanId { get; set; }
        public string ParentSpanId { get; set; }
        public int SpanType { get; set; }
        public int SpanLayer { get; set; }
        public string Component { get; set; }
        public string OperationName { get; set; }
        public double Duration { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string Peer { get; set; }
        public IList<LogDataRequest> Logs { get; } = new List<LogDataRequest>();
        public IList<KeyValuePair<string, string>> Tags { get; } = new List<KeyValuePair<string, string>>();
        public IList<TraceSegmentReferenceRequest> References { get; } = new List<TraceSegmentReferenceRequest>();

    }
    public class TraceSegmentReferenceRequest
    {
        public string TraceSegmentId { get; set; }
        public string ParentSpanId { get; set; }
        public string ParentApplicationCode { get; set; }
        public string ParentOperationName { get; set; }
        public string EntryApplicationCode { get; set; }
        public string EntryOperationName { get; set; }
        public string NetworkAddress { get; set; }
    }
    public class LogDataRequest
    {
        public DateTimeOffset Timestamp { get; set; }

        public IList<KeyValuePair<string, string>> Data { get; } = new List<KeyValuePair<string, string>>();
    }
}
