using Bucket.SkrTrace.Core.Abstractions;

namespace Bucket.SkrTrace.Core.OpenTracing
{
    public class EntrySpan : StackBasedTracingSpan
    {
        private int _currentMaxDepth;
        public EntrySpan(string spanId, string parentSpanId, string operationName) : base(spanId, parentSpanId, operationName)
        {
            _stackDepth = 0;
        }

        public override bool IsEntry => true;
        public override bool IsExit => false;
        public override ISpan Start()
        {
            if ((_currentMaxDepth = ++_stackDepth) == 1)
            {
                base.Start();
            }
            ClearWhenRestart();
            return this;
        }

        public override ISpan SetLayer(SpanLayer layer)
        {
            if (_stackDepth == _currentMaxDepth)
            {
                return base.SetLayer(layer);
            }
            return this;
        }

        public override ISpan SetComponent(SpanComponent component)
        {
            if (_stackDepth == _currentMaxDepth)
            {
                return base.SetComponent(component);
            }
            return this;
        }
        public override string OperationName
        {
            get
            {
                return base.OperationName;
            }
            set
            {
                if (_stackDepth == _currentMaxDepth)
                {
                    base.OperationName = value;
                }
            }
        }
        private void ClearWhenRestart()
        {
            _spanComponent = null;
            _layer = null;
        }
    }
}
