using Bucket.SkrTrace.Core.Abstractions;

namespace Bucket.SkrTrace.Core.Implementation.Carrier
{
    public class ContextCarrierFactory : IContextCarrierFactory
    {
        public IContextCarrier Create()
        {
            return new ContextCarrier();
        }
    }
}
