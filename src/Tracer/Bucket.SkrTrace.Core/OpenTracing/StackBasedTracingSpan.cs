namespace Bucket.SkrTrace.Core.OpenTracing
{
    public abstract class StackBasedTracingSpan : AbstractTracingSpan
    {
        protected int _stackDepth;

        protected StackBasedTracingSpan(string spanId, string parentSpanId, string operationName)
            : base(spanId, parentSpanId, operationName)
        {
            _stackDepth = 0;
        }
    }
}
