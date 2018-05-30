using System.Threading.Tasks;

namespace Bucket.OpenTracing
{
    public interface ICarrierReader
    {
        ISpanContext Read(ICarrier carrier);

        Task<ISpanContext> ReadAsync(ICarrier carrier);
    }
}