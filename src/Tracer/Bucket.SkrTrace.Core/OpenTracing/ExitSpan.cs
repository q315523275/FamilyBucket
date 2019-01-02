using Bucket.SkrTrace.Core.Abstractions;
using Bucket.SkrTrace.Core.Transport;

namespace Bucket.SkrTrace.Core.OpenTracing
{
    public class ExitSpan : StackBasedTracingSpan, IWithPeerInfo
    {
        private readonly string _peer;
        public ExitSpan(string spanId, string parentSpanId, string operationName, string peer)
            : base(spanId, parentSpanId, operationName)
        {
            _peer = peer;
        }

        public override bool IsEntry => false;

        public override bool IsExit => true;
        public string Peer => _peer;
        public override ISpan Start()
        {
            if (++_stackDepth == 1)
            {
                base.Start();
            }

            return base.Start();
        }
        public override ISpan SetLayer(SpanLayer layer)
        {
            if (_stackDepth == 1)
            {
                return base.SetLayer(layer);
            }

            return this;
        }

        public override ISpan SetComponent(SpanComponent component)
        {
            if (_stackDepth == 1)
            {
                return base.SetComponent(component);
            }

            return this;
        }

        public override string OperationName
        {
            get => base.OperationName;
            set
            {
                if (_stackDepth == 1)
                {
                    base.OperationName = value;
                }
            }
        }
        public override SpanRequest Transform()
        {
            var spanObject = base.Transform();
            spanObject.Peer = _peer;
            return spanObject;
        }
    }
}
