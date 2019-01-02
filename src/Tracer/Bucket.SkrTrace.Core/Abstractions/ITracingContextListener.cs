namespace Bucket.SkrTrace.Core.Abstractions
{
    public interface ITracingContextListener
    {
        void AfterFinished(ITraceSegment traceSegment);
    }
}
