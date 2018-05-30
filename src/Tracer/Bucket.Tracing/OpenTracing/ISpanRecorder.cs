using System.Threading.Tasks;

namespace Bucket.OpenTracing
{
    public interface ISpanRecorder
    {
        void Record(ISpan span);
    }
}