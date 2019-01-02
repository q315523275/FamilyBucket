using Bucket.SkrTrace.Core.Abstractions;
using Bucket.SkrTrace.Core.Transport;
using System;

namespace Bucket.SkrTrace.Core.Implementation
{
    public class TraceSegmentRef : ITraceSegmentRef
    {
        private readonly string _traceSegmentId;
        private readonly string _spanId;
        private readonly string _peerAddress;
        private readonly string _parentApplicationCode;
        private readonly string _parentOperationName;
        private readonly string _entryApplicationCode;
        private readonly string _entryOperationName;
        private readonly string _identity;
        public TraceSegmentRef(IContextCarrier carrier)
        {
            _traceSegmentId = carrier.TraceSegmentId;
            _spanId = carrier.SpanId;
            _parentApplicationCode = carrier.ParentApplicationCode;
            _entryApplicationCode = carrier.EntryApplicationCode;
            _peerAddress = carrier.PeerHost;
            _entryOperationName = carrier.EntryOperationName;
            _parentOperationName = carrier.ParentOperationName;
            _identity = carrier.Identity;
        }

        public string EntryOperationName => _entryOperationName;

        public string EntryApplicationCode => _entryApplicationCode;

        public bool Equals(ITraceSegmentRef other)
        {
            if (other == null)
            {
                return false;
            }

            if (other == this)
            {
                return true;
            }

            if (!(other is TraceSegmentRef segmentRef))
            {
                return false;
            }

            if (_spanId != segmentRef._spanId)
            {
                return false;
            }

            return _traceSegmentId.Equals(segmentRef._traceSegmentId);
        }

        public override bool Equals(object obj)
        {
            var other = obj as ITraceSegmentRef;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            int result = _traceSegmentId.GetHashCode();
            result = 31 * result + _spanId.GetHashCode();
            return result;
        }
        public TraceSegmentReferenceRequest Transform()
        {
            TraceSegmentReferenceRequest traceSegmentReference = new TraceSegmentReferenceRequest
            {
                TraceSegmentId = _traceSegmentId,
                ParentApplicationCode = _parentApplicationCode,
                ParentOperationName = _parentOperationName,
                ParentSpanId = _spanId,
                EntryApplicationCode = _entryApplicationCode,
                EntryOperationName = _entryOperationName,
                NetworkAddress = _peerAddress,
            };
            return traceSegmentReference;
        }
    }
}
