using Bucket.SkrTrace.Core.Abstractions;
using Bucket.SkrTrace.Core.OpenTracing;
using Bucket.SkrTrace.Core.Transport;
using Bucket.SkrTrace.Core.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Bucket.SkrTrace.Core.Implementation
{
    public class TraceSegment : ITraceSegment
    {
        private readonly IList<ITraceSegmentRef> _refs;
        private readonly IList<AbstractTracingSpan> _spans;
        private string _identity;
        public TraceSegment()
        {
            TraceSegmentId = RandomUtils.NextLong().ToString();
            _spans = new List<AbstractTracingSpan>();
            _refs = new List<ITraceSegmentRef>();
        }
        public string ApplicationCode => RuntimeEnvironment.Instance.ApplicationCode;
        public string TraceSegmentId { set; get; }
        public string Identity
        {
            set { _identity = value; }
            get { return _identity ?? RuntimeEnvironment.Instance.ApplicationCode; }
        }
        public void Archive(AbstractTracingSpan finishedSpan)
        {
            _spans.Add(finishedSpan);
        }
        public ITraceSegment Finish()
        {
            return this;
        }
        public void Ref(ITraceSegmentRef refSegment)
        {
            if (!_refs.Contains(refSegment))
            {
                _refs.Add(refSegment);
            }
        }
        public IEnumerable<ITraceSegmentRef> Refs => _refs;
        public TraceSegmentRequest Transform()
        {
            var upstreamSegment = new TraceSegmentRequest
            {
                Segment = new TraceSegmentObjectRequest
                {
                    SegmentId = TraceSegmentId,
                    ApplicationCode = ApplicationCode,
                    Identity = Identity,
                    Spans = _spans.Select(x => x.Transform()).ToArray()
                }
            };
            return upstreamSegment;
        }
    }
}
