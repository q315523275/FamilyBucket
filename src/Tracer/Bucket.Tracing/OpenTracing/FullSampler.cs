using Bucket.OpenTracing;

namespace Bucket.OpenTracing
{
    public class FullSampler : ISampler
    {
        public bool ShouldSample()
        {
            return true;
        }
    }
}