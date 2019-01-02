namespace Bucket.SkrTrace.Core.Abstractions
{
    public interface IIgnoreTracerContextListener
    {
        void AfterFinish(ITracerContext tracerContext);
    }
}
