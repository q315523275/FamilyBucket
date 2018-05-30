namespace Bucket.OpenTracing
{
    public interface ISampler
    {
        bool ShouldSample();
    }
}