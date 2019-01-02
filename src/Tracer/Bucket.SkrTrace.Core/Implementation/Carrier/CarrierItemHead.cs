using Bucket.SkrTrace.Core.Abstractions;

namespace Bucket.SkrTrace.Core.Implementation.Carrier
{
    public class CarrierItemHead : CarrierItem
    {
        public CarrierItemHead(CarrierItem next, string @namespace) : base(string.Empty, string.Empty, next, @namespace)
        {
        }
    }
}
